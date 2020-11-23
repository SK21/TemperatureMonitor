void  sendtempsBus1()
{
	if (P3 == "")
	{
		// get all sensors
		// Group 1
		DTbus1.requestTemperatures();
		delay(1000);
		OWbus1.reset_search();
		while (OWbus1.search(sensoraddress))
		{
			packetout = String(DTbus1.getTempC(sensoraddress));
			String Rom = addressTostring(sensoraddress);
			sendpacket(1, packetout, Rom);
			delay(1000);
		}
	}
	else
	{
		// get single sensor with Rom address P3
		DTbus1.requestTemperatures();
		delay(1000);
		if (SensorOnBus1()) sendpacket(1, String(SensorTemp), P3);
	}
}

void setuserdataBus1()
{
	if (SensorOnBus1())
	{
		int userdata = P2.toInt();
		DTbus1.setUserData(RomCode, userdata);        // save new data 
		DTbus1.setResolution(RomCode, resolution);    // make sure resolution is set on sensor
	}
}

void getuserdataBus1()
{
	if (SensorOnBus1())
	{
		packetout = String(DTbus1.getUserData(RomCode)); // get user data
		sendpacket(3, packetout, P3);
	}
}

boolean SensorOnBus1()
{
	//check if sensor is connected to bus, -127 means not connected
	stringToaddress(P3);
	SensorTemp = DTbus1.getTempC(RomCode);
	return (SensorTemp > -126);
}

