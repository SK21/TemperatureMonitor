
void ConnectWifi()
{
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
			ErrorCount = 0;

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

void ReadTempServer()
{
	const uint8_t MaxBuffer = 50;	// bytes
	uint16_t len = client.available();
	if (len)
	{
		byte data[MaxBuffer];
		client.read(data, MaxBuffer);
		ReadPGNs(data, len);
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
		//		- bit 2 send sensor serial # and user data
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
					if ((InCommand & 4) == 4) SendSensorDescription();
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
			if (GoodCRC(data, PGNlength))
			{

			}
		}
		break;
	}
}

void SendTemps()
{
	// temperatures
	// 0	header lo	110
	// 1	header hi	120
	// 2	module ID
	// 3-10	sensor serial #
	// 11	temp lo
	// 12	temp hi
	// 13	Remaining sensors 
	// 14	CRC

	byte PGNlength = 15;
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
		data[13] = SensorCount - i - 1;
		data[14] = CRC(data, PGNlength, 0);
		client.write(data, PGNlength);
	}
}

void SendModuleDescription()
{
	// module description
	// 0	header lo	111
	// 1	header hi	120
	// 2	module ID
	// 3-8	module mac
	// 9-18	module name
	// 19	CRC

	byte PGNlength = 20;
	byte data[PGNlength];
	data[0] = 111;
	data[1] = 120;
	data[2] = MDL.ID;
	memcpy(&data[3], MacAddr, 6);
	memcpy(&data[9], MDL.Name, 10);
	data[19] = CRC(data, PGNlength, 0);
	client.write(data, PGNlength);
}

void SendSensorDescription()
{

}



void ReadTempServerOld()
{
	int Count = 0;
	while (client.available())
	{
		char c = client.read();
		Count = Count + 1;
		if (Count > maxread) // max characters
		{
			return;
		}
		else
		{
			packetin = packetin + c;
		}
	}
}

void checkchunks()
{
	while (getchunks())  //break message up into parts, keep getting chunks from packet
	{
		processchunks();  //take action based on message
		delay(100);
	}
	packetin = "";  //erase any junk
}

boolean getchunks()
{
	int St = packetin.indexOf("^");
	if (St == -1)
	{
		// ignore message, no start character
		return false;
	}
	int D1 = packetin.indexOf("|", St + 1);
	if (D1 == -1)
	{
		return false;
	}
	int D2 = packetin.indexOf("|", D1 + 1);
	if (D2 == -1)
	{
		// no second delimiter
		return false;
	}
	int D3 = packetin.indexOf("|", D2 + 1);
	if (D3 == -1)
	{
		// no third delimiter
		return false;
	}
	P1 = packetin.substring(St + 1, D1).toInt();
	P2 = packetin.substring(D1 + 1, D2);
	P3 = packetin.substring(D2 + 1, D3);

	packetin.remove(0, D3);  // remove processed part
	return true;
}

float Tmp;
void processchunks()
{
	switch (P1)
	{
	case 0:
		// heartbeat, reset time
		pulsetime = millis();
		break;

	case 1:
		// get temperatures
		if (P3 == "")
		{
			// all sensors
			for (int i = 0; i < SensorCount; i++)
			{
				Tmp = (float)((int16_t)(Sensors[i].Temperature[1] << 8 | Sensors[i].Temperature[0]) / 16.0);
				packetout = String(Tmp);
				String Rom = addressTostring(Sensors[i].ID);
				sendpacket(1, packetout, Rom);
			}
		}
		else
		{
			// single sensor
			for (int i = 0; i < SensorCount; i++)
			{
				String Rom = addressTostring(Sensors[i].ID);
				if (P3 == Rom)
				{
					Tmp = (float)((int16_t)(Sensors[i].Temperature[1] << 8 | Sensors[i].Temperature[0]) / 16.0);
					packetout = String(Tmp);
					sendpacket(1, packetout, Rom);
				}
			}
		}
		break;

	case 2:
		//set user data
		for (int i = 0; i < SensorCount; i++)
		{
			String Rom = addressTostring(Sensors[i].ID);
			Rom.trim();
			P3.trim();
			if (P3.equals(Rom))
			{
				int userdata = P2.toInt();
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
		break;

	case 3:
		//get user data
		for (int i = 0; i < SensorCount; i++)
		{
			String Rom = addressTostring(Sensors[i].ID);
			Rom.trim();
			P3.trim();
			if (P3 == Rom)
			{
				uint16_t UserData = Sensors[i].UserData[1] << 8 | Sensors[i].UserData[0];
				packetout = String(UserData);
				sendpacket(3, packetout, Rom);
				break;
			}
		}
		break;
	}
}

void sendpacket(int id, String data, String Rom)
{
	data = "^" + String(id) + "|" + data + "|" + Rom + "|";
	client.print(data);
}

