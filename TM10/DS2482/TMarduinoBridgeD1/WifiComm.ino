

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
	int PacketSize = UDP.parsePacket();
	if (PacketSize)
	{
		int Len = UDP.read(InBuffer, 255);
		if (Len > 15)
		{
			PGN = InBuffer[0] << 8 | InBuffer[1];
			Serial.println("PGN " + String(PGN) + " received for Controlbox	" + String(InBuffer[2]));
			if (PGN == 25000)
			{
				// set sensor data
				SendEnabled = false;	// disable sending until PGN received for this controlbox
				{
					if (InBuffer[2] == Props.ID)
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

			if (PGN == 25020)
			{
				// set controlbox data
				SendEnabled = false;
				if (InBuffer[2] == Props.ID)
				{
					Serial.println("PGN " + String(PGN) + "  for this ControlBox.");
					Props.UseSleep = InBuffer[3];
					Props.SleepInterval = InBuffer[4] << 8 | InBuffer[5];
					Props.ID = InBuffer[6];
					CurrentTime = InBuffer[7] << 8 | InBuffer[8];
					Props.ControlBoxCount = InBuffer[9];
					TimeSlot = InBuffer[10];
					ReceivedReply = true;
					SaveProperties();
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

void SendData(byte SensorID, byte Status)
{
	if (SendEnabled)
	{
		// PGN 25100
		OutBuffer[0] = 98;
		OutBuffer[1] = 12;
		OutBuffer[2] = Props.ID;
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

		UDP.beginPacket(ipDestination, SendToPort);
		UDP.write(OutBuffer, sizeof(OutBuffer));
		UDP.endPacket();
		Serial.println("SendData");
	}
}

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
			Serial.println("Controlbox: " + String(Props.ID));
			Serial.print("Network: ");
			Serial.println(Props.SSID);
			Serial.print("Wifi status: ");
			Serial.println(WiFi.status());
			Serial.print("RSSI: ");
			Serial.println(WiFi.RSSI());
			Serial.print("IP: ");
			Serial.println(WiFi.localIP());
			Serial.print("UDP Destination IP: ");
			Serial.println(ipDestination);
			Serial.println();

			if (!WifiConnected())
			{
				Serial.print("Connecting to ");
				Serial.println(Props.SSID);

				WiFi.disconnect();
				delay(500);
				WiFi.mode(WIFI_STA);
				WiFi.begin(Props.SSID, Props.Password);
				delay(5000);
				ReconnectCount++;
				ConnectedCount = 0;
				Serial.print("RSSI: ");
				Serial.println(WiFi.RSSI());
				if (ReconnectCount > 10) ESP.restart();
				UDPstarted = false;
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

	if (!UDPstarted)
	{
		UDPstarted = UDP.begin(ReceiveFromPort);

		// UDP destination
		ipDestination = WiFi.localIP();
		ipDestination[3] = 255;		// change to broadcast
	}

	return WifiConnected();
}

void SendDiagnostics()
{
	if (millis() - DiagTime > 30000)
	{
		DiagTime = millis();

		// PGN 25100
		OutBuffer[0] = 98;
		OutBuffer[1] = 12;
		OutBuffer[2] = Props.ID;
		OutBuffer[15] = 4;

		OutBuffer[3] = LineUDS;
		OutBuffer[4] = LineASR;
		OutBuffer[5] = WiFi.RSSI() + 100;
		OutBuffer[6] = LineSensors;

		UDP.beginPacket(ipDestination, SendToPort);
		UDP.write(OutBuffer, sizeof(OutBuffer));
		UDP.endPacket();
		Serial.println("Send Diagnostics");
	}
}

