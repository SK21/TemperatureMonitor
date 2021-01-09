
bool WifiConnected()
{
	return ((WiFi.status() == WL_CONNECTED) && (WiFi.RSSI() > -90) && (WiFi.RSSI() != 0));
}

bool CheckWifi(int NumberTries)
{
	if ((millis() - CommTime > 5000) || NumberTries > 1)
	{
		int Count = 0;
		do
		{
			Count++;

			Serial.println();
			Serial.println("Controlbox: " + String(ControlBoxID));
			Serial.print("Network: ");
			Serial.println(WifiSSID);
			Serial.print("Wifi status: ");
			Serial.println(WiFi.status());
			Serial.print("RSSI: ");
			Serial.println(WiFi.RSSI());
			Serial.print("IP: ");
			Serial.println(WiFi.localIP());
			Serial.println();

			if (!WifiConnected())
			{
				Serial.print("Connecting to ");
				Serial.println(WifiSSID);

				WiFi.disconnect();
				delay(500);
				WiFi.begin(WifiSSID,WifiPassword);
				delay(5000);
				ReconnectCount++;
				ConnectedCount = 0;
				Serial.print("RSSI: ");
				Serial.println(WiFi.RSSI());
			}
			else
			{
				ConnectedCount++;
				ReconnectCount = 0;
			}
			Serial.println("Reconnect count: " + String(ReconnectCount));
			Serial.println("Connected count: " + String(ConnectedCount));
			Serial.println("Minutes connected: " + String(ConnectedCount * 5 / 60));
			Serial.println();
			CommTime = millis();

		} while (Count < NumberTries && !WifiConnected());
	}

	return WifiConnected();
}

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


void ReceiveData()
{
	CommandByte = 0;
	int PacketSize = UDPin.parsePacket();
	if (PacketSize)
	{
		int Len = UDPin.read(InBuffer, 255);
		if (Len > 15)
		{
			PGN = InBuffer[0] << 8 | InBuffer[1];
			Serial.println("PGN " + String(PGN) + " received for Controlbox	" + String(InBuffer[2]));
			if (PGN == 25000)
			{
				// set sensor data
				SendEnabled = false;	// disable sending until PGN received for this controlbox
				{
					if (InBuffer[2] == ControlBoxID)
					{
						Serial.println("PGN " + String(PGN) + "  for this ControlBox.");
						SendEnabled = true;
						CommandByte = InBuffer[15];
						for (byte i = 0; i < 8; i++)
						{
							SensorAddress[i] = InBuffer[i + 3];
						}
						UserData = InBuffer[11] << 8 | InBuffer[12];
					}
				}
			}

			if (PGN == 25010)
			{
				// pre-read sensors
				SendEnabled = false;
				if (InBuffer[15] == 4) UpdateSensors();
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
//15	0 - data remaining, 1 - finished, 2 - no sensors, 3 - GoToSleep, 4 - Diagnostics

void SendData(byte SensorID, byte Status)
{
	if (SendEnabled)
	{
		// PGN 25100
		OutBuffer[0] = 98;
		OutBuffer[1] = 12;
		OutBuffer[2] = ControlBoxID;
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

		UDPout.beginPacket(ipDestination, SendToPort);
		UDPout.write(OutBuffer, sizeof(OutBuffer));
		UDPout.endPacket();
		Serial.println("SendData");
	}
}

void SendDiagnostics()
{
	if(millis()-DiagTime>30000)
	{
	  DiagTime=millis();
	
	// PGN 25100
	OutBuffer[0] = 98;
	OutBuffer[1] = 12;
	OutBuffer[2] = ControlBoxID;
	OutBuffer[15] = 4;

	OutBuffer[3]=LineUDS;
	OutBuffer[4]=LineASR;
	OutBuffer[5]=WiFi.RSSI()+100;

	UDPout.beginPacket(ipDestination, SendToPort);
	UDPout.write(OutBuffer, sizeof(OutBuffer));
	UDPout.endPacket();
	Serial.println("Send Diagnostics");
	}
}
