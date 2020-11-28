// adapted from https://playground.arduino.cc/Learning/OneWire/

void UpdateSensors()
{
	Serial.println("Updating sensors");
	SensorCount = 0;
	byte Addr[8];
	for (int i = 0; i < 2; i++)
	{
		OWbus[i].reset_search();
		delay(100);
		while (OWbus[i].search(Addr) && SensorCount < 256)
		{
			SensorTemp = GetTemp(Addr, i);
			if (ValidTemp(SensorTemp))
			{
				for (int i = 0; i < 8; i++)
				{
					Sensors[SensorCount].ID[i] = Addr[i];
				}
				Sensors[SensorCount].BusID = i;
				int temp = SensorTemp * 10;
				Sensors[SensorCount].Temperature[0] = (byte)(temp >> 8);
				Sensors[SensorCount].Temperature[1] = (byte)(temp);

				Sensors[SensorCount].UserData[0] = (byte)(UserData >> 8);
				Sensors[SensorCount].UserData[1] = (byte)(UserData);
				SensorCount++;
			}
			Serial.println("Sensor count: " + String(SensorCount));
			delay(1000);
		}
	}
}

float GetTemp(byte Addr[], byte BusID)
{
	Result = -127;
	UserData = 0;

	OWbus[BusID].reset();
	OWbus[BusID].select(Addr);
	OWbus[BusID].write(0x44, 1);	// parasite power on

	delay(1000);
	OWbus[BusID].reset();
	OWbus[BusID].select(Addr);
	OWbus[BusID].write(0xBE);	// Issue read scratchpad cmd

	for (int i = 0; i < 9; i++)
	{
		dsScratchPadMem[i] = OWbus[BusID].read();
	}

	// check if valid frame
	if (dsCRC8(dsScratchPadMem, 8) == dsScratchPadMem[8])
	{
		RawTemp = (dsScratchPadMem[1] << 8) | dsScratchPadMem[0];
		Result = (float)RawTemp / 16.0;
		UserData = (dsScratchPadMem[2] << 8 | dsScratchPadMem[3]);
	}

	return Result;
}

bool ValidTemp(float Temp)
{
	return (Temp > -127);	// -127 means not connected
}

void SetUserData(byte Addr[], byte BusID, int NewValue)
{
	OWbus[BusID].reset();
	OWbus[BusID].select(Addr);
	OWbus[BusID].write(0x4E, 1);	// begin write to scratchpad

	data[0] = NewValue >> 8;
	data[1] = NewValue & 255;

	OWbus[BusID].write(data[0], 1);
	OWbus[BusID].write(data[1], 1);
	OWbus[BusID].write(9, 1);		// set resolution to 0.5°C
	delay(10);

	OWbus[BusID].reset();
	OWbus[BusID].select(Addr);

	OWbus[BusID].write(0x48, 1);	// copy scratchpad to eeprom
	delay(100);
}

byte dsCRC8(const uint8_t* addr, uint8_t len)//begins from LS-bit of LS-byte (OneWire.h)
{
	uint8_t crc = 0;
	while (len--)
	{
		uint8_t inbyte = *addr++;
		for (uint8_t i = 8; i; i--)
		{
			uint8_t mix = (crc ^ inbyte) & 0x01;
			crc >>= 1;
			if (mix) crc ^= 0x8C;
			inbyte >>= 1;
		}
	}
	return crc;
}