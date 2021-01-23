#if UseDS2482

// adapted from https://playground.arduino.cc/Learning/OneWire/

void UpdateSensors()
{
	byte Addr[8];
	byte Mem[2];
	SensorCount = 0;

	Serial.println();
	Serial.println("Searching one-wire bus... ");
	OWbus.wireResetSearch();
	delay(500);
	while (OWbus.wireSearch(Addr) && SensorCount < 256)
	{
		LineSensors++;
		if (GetTemp(Addr, Mem))
		{
			for (int i = 0; i < 8; i++)
			{
				Sensors[SensorCount].ID[i] = Addr[i];
			}
			Sensors[SensorCount].Temperature[0] = Mem[0];
			Sensors[SensorCount].Temperature[1] = Mem[1];

			Sensors[SensorCount].UserData[1] = (byte)(UserData >> 8);
			Sensors[SensorCount].UserData[0] = (byte)(UserData);
			SensorCount++;
		}
		delay(500);
	}
	Serial.println("Sensor count: " + String(SensorCount));
	LineUDS++;
}

bool GetTemp(byte Addr[], byte Mem[] )
{
	float Result = -127;
	UserData = 0;
	byte dsScratchPadMem[9];

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
			Mem[1] = dsScratchPadMem[1];
			Mem[0] = dsScratchPadMem[0];
			Result = (float)((int16_t)(Mem[1] << 8 | Mem[0]) / 16.0);	// twos complement conversion
			UserData = (dsScratchPadMem[2] << 8 | dsScratchPadMem[3]);
		}
		if (Result > -127) break;	// loop up to 3 times to get valid data
		delay(1000);
	}
	return (Result > -127);
}

void SetUserData(byte Addr[], int NewValue)
{
	OWbus.wireReset();
	OWbus.wireSelect(Addr);
	OWbus.wireWriteByte(0x4E);	// begin write to scratchpad

	OWbus.wireWriteByte(NewValue >> 8);
	OWbus.wireWriteByte(NewValue & 255);
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
#endif

