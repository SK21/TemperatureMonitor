

//PGN 25000, data received by the node :
//0			header high 97
//1 		header low 168
//2			node ID
//3 - 10	sensor address bytes 0 - 8
//11		user data high, bin number byte
//12		user data low, cable bits 7 - 4, sensor # bits 3 - 0, (0 - 15 each)
//13		-
//14		-
//15		command byte
//16		CRC8


void ReceiveData()
{
	CommandByte = 0;
	int PacketSize = UDP.parsePacket();
	if (PacketSize)
	{
		int Len = UDP.read(InBuffer, 255);
		if (Len >= CommLength)
		{
			PGN = InBuffer[0] << 8 | InBuffer[1];
			Serial.println("PGN " + String(PGN) + " received for Controlbox	" + String(InBuffer[2]));
			if (PGN == 25000)
			{
				// set sensor data
				SendEnabled = false;	// disable sending until PGN received for this controlbox
				{
					if (InBuffer[2] == BoxData.ID)
					{
						if (CRCmatch(InBuffer, CommLength))
						{
							Serial.println("PGN " + String(PGN) + "  for this ControlBox.");
							SendEnabled = true;
							CommandByte = InBuffer[15];
							for (byte i = 0; i < 8; i++)
							{
								CurrentSensorAddress[i] = InBuffer[i + 3];
							}
							UserData = InBuffer[11] << 8 | InBuffer[12];
						}
					}
				}
			}

			if (PGN == 25010)
			{
				if (CRCmatch(InBuffer, CommLength))
				{
					// pre-read sensors
					SendEnabled = false;
					if (InBuffer[15] == 4) UpdateSensors();
				}
			}

			if (PGN == 25020)
			{
				// set controlbox data
				SendEnabled = false;
				if (InBuffer[2] == BoxData.ID)
				{
					if (CRCmatch(InBuffer, CommLength))
					{
						Serial.println("PGN " + String(PGN) + "  for this ControlBox.");
						BoxData.UseSleep = InBuffer[3];
						BoxData.SleepInterval = InBuffer[4] << 8 | InBuffer[5];

						BoxData.ID = InBuffer[6];
						CurrentTime = InBuffer[7] << 8 | InBuffer[8];
						BoxData.ControlBoxCount = InBuffer[9];

						TimeSlot = InBuffer[10];
						SendDiag = InBuffer[11];
						Restart = InBuffer[12];

						ReceivedReply = true;
						SaveCBproperties();
					}
				}
			}
		}
	}
}

//PGN 25100, data sent from the controlbox :
//0	header high byte 98
//1	header low byte	12
//2	node ID byte
//3 - 10	sensor address bytes 0 - 8
//11	user data high, bin number byte
//12	user data low, cable bits 7 - 4, sensor # bits 3 - 0, (0 - 15 each)
//13	Temp high
//14	Temp low
//15	0 - data remaining, 1 - finished, 2 - no sensors, 3 - GoToSleep
//16	CRC8

void SendData(byte SensorID, byte Status)
{
	if (SendEnabled)
	{
		// PGN 25100
		OutBuffer[0] = 98;
		OutBuffer[1] = 12;
		OutBuffer[2] = BoxData.ID;
		OutBuffer[15] = Status;

		if ((Status == 0) || (Status == 1))
		{
			// report sensor
			for (byte i = 0; i < 8; i++)
			{
				OutBuffer[i + 3] = Sensors[SensorID].ID[i];
			}

			OutBuffer[11] = Sensors[SensorID].UserData[0];
			OutBuffer[12] = Sensors[SensorID].UserData[1];

			OutBuffer[13] = Sensors[SensorID].Temperature[0];
			OutBuffer[14] = Sensors[SensorID].Temperature[1];
		}

		OutBuffer[16] = CRC8(&OutBuffer[0], CommLength - 1);

		UDP.beginPacket(ipDestination, SendToPort);
		UDP.write(OutBuffer, sizeof(OutBuffer));
		UDP.endPacket();
		Serial.println("SendData");
	}
}

bool CheckWifi()
{
	if (WiFi.status() != WL_CONNECTED)
	{
		WiFi.disconnect();
		delay(500);
		WiFi.mode(WIFI_STA);
		WiFi.begin(BoxData.SSID, BoxData.Password);
		Serial.println();
		Serial.println("Connecting to Wifi");
		unsigned long WifiConnectStart = millis();

		while ((WiFi.status() != WL_CONNECTED) && ((millis() - WifiConnectStart) < 10000))
		{
			delay(500);
			Serial.print(".");
		}
		if (WiFi.status() == WL_CONNECTED)
		{
			UDP.begin(ReceiveFromPort);
			ipDestination = WiFi.localIP();
			ipDestination[3] = 255;		// change to broadcast
			Serial.println();
			Serial.println("Connected.");
			Serial.print("IP: ");
			Serial.println(WiFi.localIP());
			Serial.println("Controlbox ID: " + String(BoxData.ID));
		}
		else
		{
			Serial.println();
			Serial.println("Not connected");
		}
	}
	return (WiFi.status() == WL_CONNECTED);
}

void SendDiagnostics()
{
	if (millis() - DiagTime > 30000)
	{
		DiagTime = millis();

		// PGN 25110
		OutBuffer[0] = 98;
		OutBuffer[1] = 22;
		OutBuffer[2] = BoxData.ID;

		IPAddress ip = WiFi.localIP();
		OutBuffer[3] = ip[3];

		byte Mac[6];
		WiFi.macAddress(Mac);
		for (int i = 0; i < 6; i++)
		{
			OutBuffer[4 + i] = Mac[5 - i];	// MAC address is MAC[5],MAC[4],MAC[3],MAC[2],MAC[1],MAC[0]
		}

		OutBuffer[10] = WiFi.RSSI() + 100;

		OutBuffer[11] = LineUDS;
		OutBuffer[12] = LineASR;
		OutBuffer[13] = LineSensors;

		OutBuffer[16] = CRC8(&OutBuffer[0], CommLength - 1);

		UDP.beginPacket(ipDestination, SendToPort);
		UDP.write(OutBuffer, sizeof(OutBuffer));
		UDP.endPacket();
		Serial.println("Send Diagnostics");
	}
}

// http://www.leonardomiliani.com/en/2013/un-semplice-crc8-per-arduino/
byte CRC8(const byte* data, byte len)
{
	byte crc = 0x00;
	while (len--)
	{
		byte extract = *data++;
		for (byte tempI = 8; tempI; tempI--)
		{
			byte sum = (crc ^ extract) & 0x01;
			crc >>= 1;
			if (sum)
			{
				crc ^= 0x8C;
			}
			extract >>= 1;
		}
	}
	return crc;
}

bool CRCmatch(byte Data[], byte Len)
{
	bool Result = false;
	if (Len > 0)
	{
		byte CRC = CRC8(&Data[0], Len - 1);
		Result = (CRC == Data[Len - 1]);
	}
	return Result;
}




