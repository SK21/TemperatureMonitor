int CurrentSleepMinutes;

void GoToSleep(int SleepMinutes = 60, RFMode WakeMode = WAKE_RF_DEFAULT)
{
	if (SleepMinutes < 1)
	{
		Serial.println("Sleep off.");
	}
	else
	{
		if (SleepMinutes > 60) SleepMinutes = 60;
		Serial.println("GoToSleep " + String(SleepMinutes) + " minutes.");
		SendEnabled = true;
		SendData(0, 3);
		delay(1000);
		uint32_t SleepTime = SleepMinutes * 60 * 1000000;
		ESP.deepSleep(SleepTime, WakeMode);
	}
}

void DoSleepMode()
{
	// get sleep time left from RTC memory
	Serial.println();
	Serial.print("Sleep Mode");
	WT.MinutesRemaining = 0;
	WakeTimeData Tmp;
	system_rtc_mem_read(RTCMEMORYSTART, &Tmp, sizeof(Tmp));
	if (Tmp.Check == 200)
	{
		WT = Tmp;
	}

	if (WT.MinutesRemaining < 1)
	{
		// sleep over, connect to network and send data
		if (CheckWifi(8))
		{
			UpdateSensors();

			// loop twice to make sure
			for (int i = 0; i < 2; i++)
			{
				// send data
				SendEnabled = true;
				AllSensorsReport();

				// wait for reply
				SendTime = millis();
				do
				{
					ReceivedReply = false;
					ReceiveData();
					if (ReceivedReply) break;
				} while (millis() - SendTime < 5000);	// wait 5 seconds for reply
				if (ReceivedReply) break;
			}
		}
		// calculate next sleep interval
		WT.MinutesRemaining = MinutesLeft();
	}

	if (Props.UseSleep)	// check if updated from reply
	{
		CurrentSleepMinutes = WT.MinutesRemaining;

		// decrement sleep minutes
		WT.MinutesRemaining = WT.MinutesRemaining - 60;
		if (WT.MinutesRemaining < 0) WT.MinutesRemaining = 0;

		// save time left to RTC memory
		WT.Check = 200;
		system_rtc_mem_write(RTCMEMORYSTART, &WT, sizeof(WT));

		if (WT.MinutesRemaining > 0)
		{
			// wake with wifi off
			GoToSleep(CurrentSleepMinutes, WAKE_RF_DISABLED);
		}
		else
		{
			// last sleep, wake with wifi on
			GoToSleep(CurrentSleepMinutes);
		}
	}
}

int MinutesLeft()
{
	// Each controlbox has an exclusive timeslot to send data within each
	// sleep interval. The sleep intervals are over the course of one day,
	// 24 hrs, or 0-1439 minutes.
	// Example with a 15 minute timeslot and a 6 hour sleep interval
	// and the current time is 750 minutes:
	// Controlbox 0 first timeslot is 0-14 minutes in the first report cycle.
	// Each cycle adds the greater of sleep interval or controlbox count * timeslot to the start time.
	// Move to the next cycle where start time is > current time.
	// In this way each controlbox has an exclusive 15 minutes to send the data with
	// no interference from other controlboxes.

	int CycleCount = 0;
	int StartTime = 0;
	int Result = 0;
	int AddTime = Props.ControlBoxCount * TimeSlot;
	if (AddTime < Props.SleepInterval) AddTime = Props.SleepInterval;
	AddTime += 1;

	do
	{
		StartTime = (CycleCount * AddTime) + Props.ID * TimeSlot;
		CycleCount++;
	} while (StartTime < CurrentTime);

	Result = StartTime - CurrentTime;

	if (CurrentTime + Result > 1439)
	{
		// next day, start over
		Result = (1439 - CurrentTime) + Props.ID * TimeSlot;
	}

	return Result;
}

