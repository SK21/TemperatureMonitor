
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

void DoUpdate()
{
	UpdateTemps();
	server.send(200, "text/html", GetPage0());
}

void handleCredentials()
{
	int NewID;
	int Interval;

	String Val = server.arg("prop1");
	Val.trim();	// remove leading and trailing spaces
	Val.toCharArray(MDLnetwork.SSID, sizeof(MDLnetwork.SSID) - 1);

	Val = server.arg("prop2");
	Val.trim();
	Val.toCharArray(MDLnetwork.Password, sizeof(MDLnetwork.Password) - 1);

	Val = server.arg("prop4");
	Val.trim();
	Val.toCharArray(MDL.Name, sizeof(MDL.Name) - 1);

	if (server.hasArg("prop5"))
	{
		MDLnetwork.UseWifi = true;
		Serial.println("Using wifi.");
	}
	else
	{
		MDLnetwork.UseWifi = false;
		WiFi.disconnect(true);
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

	SaveData();
}
