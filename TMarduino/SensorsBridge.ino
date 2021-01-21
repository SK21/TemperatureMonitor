#if UseDS2482

// adapted from https://playground.arduino.cc/Learning/OneWire/

byte MSB;
byte LSB;

void UpdateSensors()
{
	SensorCount = 0;
	byte Addr[8];
	Serial.println();
	Serial.println("Searching one-wire bus... ");
	OWbus.wireResetSearch();
	delay(500);
	while (OWbus.wireSearch(Addr) && SensorCount < 256)
	{
		SensorTemp = GetTemp(Addr);
		if (ValidTemp(SensorTemp))
		{
			for (int i = 0; i < 8; i++)
			{
				Sensors[SensorCount].ID[i] = Addr[i];
			}
			Sensors[SensorCount].Temperature[0] = MSB;
			Sensors[SensorCount].Temperature[1] = LSB;

			Sensors[SensorCount].UserData[0] = (byte)(UserData >> 8);
			Sensors[SensorCount].UserData[1] = (byte)(UserData);
			SensorCount++;
		}
		delay(500);
	}
	Serial.println("Sensor count: " + String(SensorCount));
	LineUDS++;
}

float GetTemp(byte Addr[])
{
	Result = -127;
	UserData = 0;
	for (int i = 0; i < 3; i++)
	{
		OWbus.wireReset();
		OWbus.wireSelect(Addr);
		OWbus.wireWriteByte(0x44);

		delay(1000);
		OWbus.wireReset();
		OWbus.wireSelect(Addr);
		OWbus.wireWriteByte(0xBE);	// Issue read scratchpad cmd
		for (int i = 0; i < 9; i++)
		{
			dsScratchPadMem[i] = OWbus.wireReadByte();
		}

		// check if valid frame
		if (OWbus.crc8(dsScratchPadMem, 8) == dsScratchPadMem[8])
		{
			MSB = dsScratchPadMem[1];
			LSB = dsScratchPadMem[0];
			Result = FromTwos(MSB, LSB);
			UserData = (dsScratchPadMem[2] << 8 | dsScratchPadMem[3]);
		}
		if (Result > -127) break;	// loop up to 3 times to get valid data
		delay(1000);
	}
	return Result;
}

bool ValidTemp(float Temp)
{
	return (Temp > -127);	// -127 means not connected
}

void SetUserData(byte Addr[], int NewValue)
{
	OWbus.wireReset();
	OWbus.wireSelect(Addr);
	OWbus.wireWriteByte(0x4E);	// begin write to scratchpad

	data[0] = NewValue >> 8;
	data[1] = NewValue & 255;

	OWbus.wireWriteByte(data[0]);
	OWbus.wireWriteByte(data[1]);
	OWbus.wireWriteByte(9);		// set resolution to 0.5C
	delay(10);

	OWbus.wireReset();
	OWbus.wireSelect(Addr);

	OWbus.wireWriteByte(0x48);	// copy scratchpad to eeprom
	delay(100);
}

void AllSensorsReport()
{
	if (SensorCount == 0)
	{
		// report no sensors
		SendData(0, 2);
	}
	else
	{
		// report available sensors
		bool Last;
		for (byte i = 0; i < SensorCount; i++)
		{
			Last = ((i + 1) == SensorCount);
			SendData(i, Last);
			delay(150);
		}
	}

	LineASR++;
}

void SingleSensorReport()
{
	for (byte i = 0; i < SensorCount; i++)
	{
		if (memcmp(Sensors[i].ID, CurrentSensorAddress, 8) == 0)
		{
			SendData(i, 1);
			break;	// exit loop
		}
	}
}

void SetNewUserData()
{
	for (byte i = 0; i < SensorCount; i++)
	{
		if (memcmp(Sensors[i].ID, CurrentSensorAddress, 8) == 0)
		{
			SetUserData(CurrentSensorAddress, UserData);
			UpdateSensors(); // to update array
			break;	// exit loop
		}
	}
}

float FromTwos(byte MSB, byte LSB)
{
	float r = 0;
	uint16_t t = 0;
	uint16_t s = MSB << 8 | LSB;
	if ((s & 32768) == 32768)
	{
		// negative number
		t = ~s + 1; // complement + 1
		r = t * -1.0; // multiply by -1 to result in two's complement
		r = (float)(r / 16.0); //convert one-wire temperature
	}
	else
	{
		// positive number
		r = (float)(s / 16.0);
	}
	return r;
}

#endif

