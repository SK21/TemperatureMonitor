
bool CheckWifi()
{
	if (WiFi.status() != WL_CONNECTED)
	{
		WiFi.disconnect();
		delay(500);
		WiFi.mode(WIFI_AP_STA);

		WiFi.begin(MDL.SSID, MDL.Password);
		Serial.println();
		Serial.println("Connecting to Wifi");
		unsigned long WifiConnectStart = millis();

		while ((WiFi.status() != WL_CONNECTED) && ((millis() - WifiConnectStart) < 15000))
		{
			delay(500);
			Serial.print(".");
		}
		if (WiFi.status() == WL_CONNECTED)
		{
			Serial.println();
			Serial.println("Connected.");
			Serial.print("IP: ");
			Serial.println(WiFi.localIP());
		}
		else
		{
			Serial.println();
			Serial.println("Not connected");
		}
	}
	return (WiFi.status() == WL_CONNECTED);
}

int ServerCount = 0;
void checkserver()
{
	if (millis() - pulsetime > pulsewait)
	{
		// server not connected
		if (ServerCount == 0)
		{
			Serial.print("Connecting to server at ");
			Serial.print(IPtoString(MDL.ServerIP));
			Serial.println(" ...");
		}

		Serial.print(".");
		ServerCount++;

		if (ServerCount > 11)
		{
			Serial.println("");
			Serial.println("Server not connected.");
			//WiFi.disconnect();
			ServerCount = 0;
		}
		else
		{
			if (client.connect(MDL.ServerIP, MDL.Port))
			{
				// server connected
				pulsetime = millis();
				Serial.println("");
				Serial.println("Server connected.");
				ServerCount = 0;
			}
		}
	}
}

void readserver()
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
		if (DS2842Connected)
		{
			UpdateSensorsMaster();
		}
		else
		{
			UpdateSensors();
		}

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

