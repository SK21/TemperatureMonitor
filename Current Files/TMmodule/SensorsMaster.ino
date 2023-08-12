
// adapted from https://playground.arduino.cc/Learning/OneWire/

void UpdateSensorsMaster()
{
	byte Addr[8];
	byte Mem[2];
	SensorCount = 0;

	Serial.println();
	Serial.println("Searching one-wire bus... ");
	OneWireMaster.wireResetSearch();
	delay(500);
	while (OneWireMaster.wireSearch(Addr) && SensorCount < 256)
	{
		if (GetTempMaster(Addr, Mem))
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
}

bool GetTempMaster(byte Addr[], byte Mem[])
{
	delay(500);
	float Result = -127.0;
	UserData = 0;
	byte dsScratchPadMem[9];

	for (int i = 0; i < 3; i++)
	{
		OneWireMaster.wireReset();
		OneWireMaster.wireSelect(Addr);
		OneWireMaster.wireWriteByte(0x44);  

		delay(1000);
		OneWireMaster.wireReset();
		OneWireMaster.wireSelect(Addr);
		OneWireMaster.wireWriteByte(0xBE);	// Issue read scratchpad cmd
		for (int i = 0; i < 9; i++)
		{
			dsScratchPadMem[i] = OneWireMaster.wireReadByte();
		}

		// check if valid frame
		if (OneWireMaster.crc8(dsScratchPadMem, 8) == dsScratchPadMem[8])
		{
			Mem[1] = dsScratchPadMem[1];	// msb
			Mem[0] = dsScratchPadMem[0];	// lsb
			Result = (float)((int16_t)(Mem[1] << 8 | Mem[0]) / 16.0);	// twos complement conversion
			UserData = (dsScratchPadMem[2] << 8 | dsScratchPadMem[3]);
		}
		if (Result > -127) break;	// loop up to 3 times to get valid data
		delay(1000);
	}
	return (Result > -127);
}

void SetUserDataMaster(byte Addr[], int NewValue)
{
	delay(500);
	OneWireMaster.wireReset();
	OneWireMaster.wireSelect(Addr);
	OneWireMaster.wireWriteByte(0x4E);	// begin write to scratchpad

	OneWireMaster.wireWriteByte(NewValue >> 8);
	OneWireMaster.wireWriteByte(NewValue & 255);
	OneWireMaster.wireWriteByte(9);		// set resolution to 0.5C
	delay(10);

	OneWireMaster.wireReset();
	OneWireMaster.wireSelect(Addr);

	OneWireMaster.wireWriteByte(0x48);	// copy scratchpad to eeprom
	delay(100);
}

