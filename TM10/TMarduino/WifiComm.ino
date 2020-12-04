
void CheckWifi()
{
	if (millis() - CommTime > 5000)
	{
		Serial.println();
		ConnectionStatus = WiFi.status();
		Serial.println("Wifi status: " + String(ConnectionStatus));
		Serial.print("RSSI: ");
		Serial.println(WiFi.RSSI());

		if ((ConnectionStatus != WL_CONNECTED) || (WiFi.RSSI() <= -90) || (WiFi.RSSI() == 0))
		{
			Serial.print("Connecting to ");
			Serial.println(NetworkSSID);

			ConnectionStatus = WiFi.begin(NetworkSSID, NetworkPassword);
			delay(5000);
			ReconnectCount++;
			ConnectedCount = 0;
			Serial.print("RSSI: ");
			Serial.println(WiFi.RSSI());
			if (ReconnectCount > 5) ESP.restart();	// restart with wifi manager
		}
		else
		{
			ConnectedCount++;
			ReconnectCount = 0;
		}
		Serial.println("Reconnect count: " + String(ReconnectCount));
		Serial.println("Connected count: " + String(ConnectedCount));
		Serial.println("Minutes connected: " + String(ConnectedCount * 5 / 60));
		CommTime = millis();
	}
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
	int PacketSize = UDP.parsePacket();
	if (PacketSize)
	{
		int Len = UDP.read(InBuffer, 255);
		if (Len > 15)
		{
			if (InBuffer[0] == 97 && InBuffer[1] == 168)
			{
				Serial.println("PGN received.");
				SendEnabled = false;
				if (InBuffer[15] == 4)
				{
					// pre-read sensors
					UpdateSensors();
				}
				else
				{
					if (InBuffer[2] == ControlBoxID)
					{
						Serial.println("PGN for this ControlBox.");
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
		}
	}
}

//PGN 25100, data sent from the node :
//0	header high byte 98
//1	header low byte	12
//2	node ID byte
//3 - 10	sensor address bytes 0 - 8
//11	user data high, bin number byte
//12	user data low, cable bits 7 - 4, sensor # bits 3 - 0, (0 - 15 each)
//13	Temp high
//14	Temp low
//15	previously received command

void SendData(byte SensorID)
{
	if (SendEnabled)
	{
		OutBuffer[0] = 98;
		OutBuffer[1] = 12;
		OutBuffer[2] = ControlBoxID;

		for (byte i = 0; i < 8; i++)
		{
			OutBuffer[i + 3] = Sensors[SensorID].ID[i];
		}

		OutBuffer[11] = Sensors[SensorID].UserData[0];
		OutBuffer[12] = Sensors[SensorID].UserData[1];

		OutBuffer[13] = Sensors[SensorID].Temperature[0];
		OutBuffer[14] = Sensors[SensorID].Temperature[1];

		OutBuffer[15] = CommandByte;

		UDP.beginPacket(ipDestination, SendToPort);
		UDP.write(OutBuffer, sizeof(OutBuffer));
		UDP.endPacket();
	}
}


