void  sendtempsBus2()
{
	if (P3 == "")
	{
		// get all sensors
		// Group 2
		DTbus2.requestTemperatures();
		delay(1000);
		OWbus2.reset_search();
		while (OWbus2.search(sensoraddress))
		{
			packetout = String(DTbus2.getTempC(sensoraddress));
			String Rom = addressTostring(sensoraddress);
			sendpacket(1, packetout, Rom);
			delay(1000);
		}
	}
	else
	{
		// get single sensor with Rom address P3
		DTbus2.requestTemperatures();
		delay(1000);
		if (SensorOnBus2()) sendpacket(1, String(SensorTemp), P3);
	}
}

void setuserdataBus2()
{
	if (SensorOnBus2())
	{
		int userdata = P2.toInt();
		DTbus2.setUserData(RomCode, userdata);        // save new data 
		DTbus2.setResolution(RomCode, resolution);    // make sure resolution is set on sensor
	}
}

void getuserdataBus2()
{
	if (SensorOnBus2())
	{
		packetout = String(DTbus2.getUserData(RomCode)); // get user data
		sendpacket(3, packetout, P3);
	}
}

boolean SensorOnBus2()
{
	//check if sensor is connected to bus, -127 means not connected
	stringToaddress(P3);
	SensorTemp = DTbus2.getTempC(RomCode);
	return (SensorTemp > -126);
}
