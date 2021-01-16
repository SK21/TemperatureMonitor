void DoClientMode()
{
	FlashLED();

	CheckWifi(1);

	if ((ConnectedCount == 1) && !SensorsCounted)
	{
		UpdateSensors();	// get an initial count of connected sensors
		SensorsCounted = true;
	}

	ReceiveData();

	switch (CommandByte)
	{
	case 1:
		// all sensors report
		AllSensorsReport();
		break;
	case 2:
		// specific sensor report
		SingleSensorReport();
		break;
	case 3:
		// set sensor userdata
		SetNewUserData();
		break;
	}
}

void FlashLED()
{
	if ((millis() - FlashTime > 1000))
	{
		FlashTime = millis();
		FlashState = !FlashState;
		if (FlashState)
		{
			digitalWrite(BUILTIN_LED, HIGH);
		}
		else
		{
			digitalWrite(BUILTIN_LED, LOW);
		}
	}
}
