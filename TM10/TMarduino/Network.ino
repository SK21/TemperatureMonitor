unsigned long ConnectTimeOut = 300000;
unsigned long ConnectStart;

void NetConnect()
{
	WiFi.disconnect();
	delay(1000);

	LoadProperties();

	bool StartWebPage = false;

	if (UseWebPage)
	{
		StartWebPage = true;
	}
	else
	{
		StartWebPage = !CheckWifi(8);
	}

	if (StartWebPage)
	{
		Serial.println();
		Serial.println("Starting web page");

		server.on("/", handleRoot);
		server.on("/Temps", handleTemps);
		server.onNotFound(handleRoot);

		server.begin();

		String AP = "TempMonitor " + String(Props.ID);
		WiFi.mode(WIFI_AP);
		WiFi.softAP(AP);

		ConnectStart = millis();
		while (millis() - ConnectStart < ConnectTimeOut)
		{
			server.handleClient();
		}
		ESP.restart();
	}
}

void handleRoot()
{
	Serial.println("HandleRoot");

	if (server.hasArg("prop1"))
	{
		handleSubmit();
	}
	else
	{
		server.send(200, "text/html", GetPage1());
	}
}

void handleSubmit()
{
	Serial.println("handleSubmit");

	int NewID;
	int Interval;

	server.arg("prop1").toCharArray(Props.SSID, sizeof(Props.SSID) - 1);
	server.arg("prop2").toCharArray(Props.Password, sizeof(Props.Password) - 1);

	NewID = server.arg("prop3").toInt();
	Interval = server.arg("prop4").toInt();

	if (NewID >= 0 && NewID <= 255)Props.ID = NewID;
	if (Interval >= 0 && Interval <= 1439)Props.SleepInterval = Interval;

	server.send(200, "text/html", GetPage1());
	SaveProperties();
	delay(3000);

	ESP.restart();
}

void LoadProperties()
{
	Serial.println("Loading Properties.");
	Properties tmp;
	EEPROM.begin(512);
	EEPROM.get(0, tmp);
	if (tmp.Check == 250)
	{
		Props = tmp;
	}
	Serial.println("Sleep property Load " + String(Props.UseSleep));
}

void SaveProperties()
{
	Props.Check = 250;
	EEPROM.begin(512);
	EEPROM.put(0, Props);
	if (EEPROM.commit())
	{
		Serial.println("Properties saved.");
	}
	else
	{
		Serial.println("Properties not saved.");
	}
	Serial.println("Sleep property Save " + String(Props.UseSleep));
}



