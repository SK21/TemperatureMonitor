
void HandleRoot()
{
	if (server.hasArg("prop1"))
	{
		handleCredentials();
	}
	else
	{
		server.send(200, "text/html", GetPage0());
	}
}

void HandlePage2()
{
	// network
	server.send(200, "text/html", GetPage2());
}

char ServerIP[32];
void handleCredentials()
{
	int NewID;
	int Interval;

	server.arg("prop1").toCharArray(MDL.SSID, sizeof(MDL.SSID) - 1);
	server.arg("prop2").toCharArray(MDL.Password, sizeof(MDL.Password) - 1);
	server.arg("prop3").toCharArray(ServerIP, sizeof(ServerIP) - 1);
	server.arg("prop4").toCharArray(MDL.Name, sizeof(MDL.Name) - 1);

	if (server.hasArg("prop5"))
	{
		MDL.UseWifi = true;
		Serial.println("Using wifi.");
	}
	else
	{
		MDL.UseWifi = false;
	}

	if (server.hasArg("prop6"))
	{
		MDL.UseDS2482 = true;
		Serial.println("Using DS2482.");
	}
	else
	{
		MDL.UseDS2482 = false;
	}

	if (server.hasArg("prop7"))
	{
		MDL.StrongPullup = true;
		Serial.println("Using strong pullup.");
	}
	else
	{
		MDL.StrongPullup = false;
	}

	server.send(200, "text/html", GetPage0());

	if (MDL.ServerIP.fromString(ServerIP))
	{
		Serial.print("New Server IP: ");
		Serial.println(IPtoString(MDL.ServerIP));
	}
	else
	{
		Serial.println("ServerIP was invalid.");
	}

	EEPROM.begin(512);
	EEPROM.put(0, InoID);
	EEPROM.put(10, MDL);
	EEPROM.commit();

	delay(3000);

	ESP.restart();
}
