
void sendtempsBusP()
{
	if (P3 == "")
	{
		//read all sensors
		byte Addr[8];
		byte data[9];
		byte i;
		OWbusP.reset_search();
		while (OWbusP.search(Addr))
		{
			OWbusP.reset();
			OWbusP.select(Addr);
			OWbusP.write(0x44, 1);	//begin conversion with parasitic power
			delay(1000);
			OWbusP.reset();
			OWbusP.select(Addr);
			OWbusP.write(0xBE);	//begin read scratchpad
			for (i = 0; i < 9; i++)
			{
				data[i] = OWbusP.read();
			}
			int16_t raw = (data[1] << 8) | data[0];
			byte cfg = (data[4] & 0x60);
			// at lower res, the low bits are undefined, so let's zero them
			if (cfg == 0x00) raw = raw & ~7;  // 9 bit resolution, 93.75 ms
			else if (cfg == 0x20) raw = raw & ~3; // 10 bit res, 187.5 ms
			else if (cfg == 0x40) raw = raw & ~1; // 11 bit res, 375 ms
			//// default is 12 bit resolution, 750 ms conversion time
			float Celsius = (float)raw / 16.0;
			String Rom = addressTostring(Addr);
			sendpacket(1, String(Celsius), Rom);
		}
	}
	else
	{
		//read specified sensor
		if (SensorOnBusP())
		{
			byte data[9];
			byte i;
			OWbusP.reset();
			OWbusP.select(RomCode);
			OWbusP.write(0x44, 1);	//begin conversion with parasitic power
			delay(1000);
			OWbusP.reset();
			OWbusP.select(RomCode);
			OWbusP.write(0xBE);	//begin read scratchpad
			for (i = 0; i < 9; i++)
			{
				data[i] = OWbusP.read();
			}
			int16_t raw = (data[1] << 8) | data[0];
			byte cfg = (data[4] & 0x60);
			// at lower res, the low bits are undefined, so let's zero them
			if (cfg == 0x00) raw = raw & ~7;  // 9 bit resolution, 93.75 ms
			else if (cfg == 0x20) raw = raw & ~3; // 10 bit res, 187.5 ms
			else if (cfg == 0x40) raw = raw & ~1; // 11 bit res, 375 ms
			//// default is 12 bit resolution, 750 ms conversion time
			float Celsius = (float)raw / 16.0;
			sendpacket(1, String(Celsius), P3);
		}
	}

}

void setuserdataBusP()
{
	if (SensorOnBusP())
	{
		String Rom = addressTostring(RomCode);
		int userdata = P2.toInt();	//convert received packet value to integer
		byte TH = userdata & 255;
		byte TL = userdata >> 8;
		OWbusP.reset();
		OWbusP.select(RomCode);
		OWbusP.write(0x4E,1);	//begin write to scratchpad
		OWbusP.write(TL,1);
		OWbusP.write(TH,1);
		OWbusP.write(resolution,1);
		delay(1000);
		OWbusP.write(0x48,1);		//copy scratchpad to eeprom
		delay(20);
	}
}

void getuserdataBusP()
{
	if (SensorOnBusP())
	{
		byte data[9];
		byte i;
		int UserData;
		PowerUp();
		OWbusP.reset();
		OWbusP.select(RomCode);
		OWbusP.write(0xBE,1);	//begin read scratchpad
		for (i = 0; i < 9; i++)
		{
			data[i] = OWbusP.read();
		}
		UserData = (data[2] << 8) | data[3];
		sendpacket(3, String(UserData), P3);		
	}
}

boolean SensorOnBusP()
{
	byte Addr[8];
	boolean Found;
	byte i;
	stringToaddress(P3);	// get sensor address requested, stored in variable array "RomCode"
	OWbusP.reset_search();
	while (OWbusP.search(Addr))
	{
		Found = true;
		for (i = 0; i < 8; i++)
		{
			Found = Found && (Addr[i] == RomCode[i]);
		}
		if (Found) return true;
	}
	return false;
}

void PowerUp()
{
	OWbusP.write(0x44, 1);	
	delay(1000);
}
