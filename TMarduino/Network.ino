unsigned long ConnectTimeOut = 300000;
unsigned long ConnectStart;

void NetConnect()
{
	WiFi.disconnect();
	delay(1000);

	LoadCBproperties();

	bool StartWebPage = false;

	if (ForceWebPage)
	{
		StartWebPage = true;
	}
	else
	{
		StartWebPage = !CheckWifi();
	}

	if (StartWebPage)
	{
		Serial.println();
		Serial.println("Starting web page");

		server.on("/", handleRoot);
		server.on("/Temps", handleTemps);
		server.onNotFound(handleRoot);

		server.begin();

		String AP = "TempMonitor " + String(BoxData.ID);
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

	server.arg("prop1").toCharArray(BoxData.SSID, sizeof(BoxData.SSID) - 1);
	server.arg("prop2").toCharArray(BoxData.Password, sizeof(BoxData.Password) - 1);

	NewID = server.arg("prop3").toInt();
	Interval = server.arg("prop4").toInt();

	if (NewID >= 0 && NewID <= 255)BoxData.ID = NewID;
	if (Interval >= 0 && Interval <= 1439)BoxData.SleepInterval = Interval;

	server.send(200, "text/html", GetPage1());
	SaveCBproperties();
	delay(3000);

	ESP.restart();
}

void LoadCBproperties()
{
	Serial.println("Loading CBproperties.");
	ControlBoxData tmp;
	EEPROM.begin(512);
	EEPROM.get(0, tmp);
	if (tmp.Check == 250)
	{
		BoxData = tmp;
	}
}

void SaveCBproperties()
{
	BoxData.Check = 250;
	EEPROM.begin(512);
	EEPROM.put(0, BoxData);
	if (EEPROM.commit())
	{
		Serial.println("CBproperties saved.");
	}
	else
	{
		Serial.println("CBproperties not saved.");
	}
}



