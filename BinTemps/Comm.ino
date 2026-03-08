
void ConnectWifi()
{
	uint8_t ErrorCount;
	Serial.println("");
	Serial.print("Connecting to ");
	Serial.println(MDL.SSID);
	WiFi.mode(WIFI_AP_STA);
	WiFi.begin(MDL.SSID, MDL.Password);
	ErrorCount = 0;
	while (WiFi.status() != WL_CONNECTED)
	{
		delay(1000);
		Serial.print(".");
		if (ErrorCount++ > 15) break;
	}

	if (WiFi.status() == WL_CONNECTED)
	{
		Serial.println("");
		Serial.println("WiFi connected");
		Serial.println("IP address: ");
		Serial.println(WiFi.localIP());
		Serial.printf("RSSI: %d dBm\n", WiFi.RSSI());
		Serial.println("");

		//WiFi.setAutoReconnect(true);
		//WiFi.persistent(true);
	}
	else
	{
		Serial.println("");
		Serial.println("WiFi not connected");
		Serial.println("");

		WiFi.disconnect(true);	// prevent auto-reconnect
	}
}

void CheckTempServer()
{

	ServerConnected = pulsewait > (millis() - pulsetime);
	if (!ServerConnected)
	{
		// check if time between connection attempts is greater than setting
		if (millis() - LastServerConnectTime > ServerConnectInterval)
		{
			LastServerConnectTime = millis();
			uint8_t ErrorCount = 0;

			Serial.print("Connecting to server at ");
			Serial.print(IPtoString(MDL.ServerIP));
			Serial.println(" ...");

			while (!ServerConnected)
			{
				ServerConnected = client.connect(MDL.ServerIP, MDL.Port);
				delay(1000);
				Serial.print(".");
				if (ErrorCount++ > 5) break;
			}

			if (ServerConnected)
			{
				pulsetime = millis();
				Serial.println("");
				Serial.println("Server connected.");
			}
			else
			{
				Serial.println("");
				Serial.println("Server not connected.");
			}
		}
	}
}

void ReceiveData()
{
	const uint16_t MaxBuffer = 50;	// bytes
	uint16_t len = client.available();
	if (len)
	{
		static byte data[MaxBuffer];
		uint16_t BytesRead = client.read(data, MaxBuffer);
		if (BytesRead > 2) ReadPGNs(data, BytesRead);
	}
}

void ReadPGNs(byte data[], uint16_t len)
{
	uint16_t PGN = data[1] << 8 | data[0];
	byte PGNlength;
	switch (PGN)
	{
	case 30820:
		// heartbeat
		// 0	header lo	100
		// 1	header hi	120
		// 2	crc

		PGNlength = 3;
		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				pulsetime = millis();
			}
		}
		break;

	case 30821:
		// command
		// 0	header lo	101
		// 1	header hi	120
		// 2	module ID
		// 3	command
		//		- bit 0	send sensor temps
		//		- bit 1	send module description
		// 4	crc

		PGNlength = 5;
		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				byte InCommand = data[3];
				if ((InCommand & 1) == 1) SendTemps();

				if (data[2] == MDL.ID)
				{
					if ((InCommand & 2) == 2) SendModuleDescription();
				}
			}
		}
		break;

	case 30822:
		// set module description
		// 0	header lo	102
		// 1	header hi	120
		// 2-7	module mac
		// 8	new module ID
		// 9-18	new module name
		// 19	crc

		PGNlength = 20;
		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				if (memcmp(&data[2], &MacAddr[0], 6) == 0)
				{
					MDL.ID = data[8];
					memcpy(MDL.Name, &data[9], 10);
					SaveData();
				}
			}
		}
		break;

	case 30823:
		// set sensor description
		// 0	header lo	103
		// 1	header hi	120
		// 2	module ID
		// 3-10	sensor serial #
		// 11	user data, byte 0
		// 12	user data, byte 1
		// 13	CRC

		PGNlength = 14;
		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength) && data[2] == MDL.ID)
			{
				for (int i = 0; i < SensorCount; i++)
				{
					if (memcmp(&data[3], &Sensors[i].ID[0], 8) == 0)
					{
						int userdata = data[11] | (data[12] << 8);
						if (DS2842Connected)
						{
							SetUserDataMaster(Sensors[i].ID, userdata);
						}
						else
						{
							SetUserData(Sensors[i].ID, Sensors[i].BusID, userdata);
						}
						break;
					}
				}
			}
		}
		break;
	}
}

void SendTemps()
{
	// PGN 30830 temperatures
	// 0	header lo	110
	// 1	header hi	120
	// 2	module ID
	// 3-10	sensor serial #
	// 11	temp lo
	// 12	temp hi
	// 13	user data 0
	// 14	user data 1
	// 15	Remaining sensors 
	// 16	CRC

	byte PGNlength = 17;
	byte data[PGNlength];

	for (int i = 0; i < SensorCount; i++)
	{
		memset(data, 0, PGNlength);
		data[0] = 110;
		data[1] = 120;
		data[2] = MDL.ID;

		memcpy(&data[3], Sensors[i].ID, 8);

		data[11] = Sensors[i].Temperature[0];
		data[12] = Sensors[i].Temperature[1];
		data[13] = Sensors[i].UserData[0];
		data[14] = Sensors[i].UserData[1];
		data[15] = SensorCount - i - 1;

		data[16] = CRC(data, PGNlength, 0);
		client.write(data, PGNlength);
	}
}

void SendModuleDescription()
{
	// PGN 30831 module description
	// 0		header lo	111
	// 1		header hi	120
	// 2		module ID
	// 3-8		module mac
	// 9-18		module name
	// 19-20	InoID
	// 21		CRC

	byte PGNlength = 22;
	byte data[PGNlength];
	data[0] = 111;
	data[1] = 120;
	data[2] = MDL.ID;
	memcpy(&data[3], MacAddr, 6);
	memcpy(&data[9], MDL.Name, 10);
	data[19] = (byte)InoID;
	data[20] = (byte)(InoID >> 8);

	data[21] = CRC(data, PGNlength, 0);
	client.write(data, PGNlength);
}
