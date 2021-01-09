int CurrentSleepMinutes;

void GoToSleep(int SleepMinutes = 60, RFMode WakeMode = WAKE_RF_DEFAULT)
{
	Serial.println("GoToSleep " + String(SleepMinutes) + " minutes.");
	SendData(0, 3);
	delay(100);
	if (SleepMinutes > 60) SleepMinutes = 60;
	if (SleepMinutes < 0) SleepMinutes = 0;
	uint32_t SleepTime = SleepMinutes * 60 * 1000000;
	ESP.deepSleep(SleepTime, WakeMode);
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
				} while (millis() - SendTime < 3000);	// wait 3 seconds for reply
				if (ReceivedReply) break;
			}
		}
		// calculate next sleep interval
		WT.MinutesRemaining = MinutesLeft();
	}

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

int MinutesLeft()
{
	// Each controlbox has an exclusive timeslot to send data within each
	// sleep interval. The sleep intervals are over the course of one day,
	// 24 hrs, or 0-1439 minutes.
	// Example with a 15 minute timeslot and a 6 hour sleep interval
	// and the current time is 750 minutes:
	// Controlbox 0 first timeslot is 0-14 minutes in the first sleep
	// interval (0-360 minutes). This is less the current time so we move to the next
	// interval that is greater than the current time. The fourth interval (1081-1420) is
	// used. Controlbox 0 timeslot is 1081-1095. The current time 750 is subtracted
	// from 1081. This leaves 231 minutes of sleep remaining.
	// For Controlbox 1 the timeslot is 1096-1110. Controlbox 1 sleep remaining is 246.
	// In this way each controlbox has an exclusive 15 minutes to send the data with
	// no interference from other controlboxes.
	int Result = 0;
	int Interval = Props.SleepInterval;
	if (CurrentTime < 0 || CurrentTime > 1439) CurrentTime = 0;
	int IntervalCount = 0;
	int StartTime = 0;
	do
	{
		StartTime = IntervalCount * Interval + (Props.ID * TimeSlot);
		IntervalCount++;
	} while ((StartTime < CurrentTime) && (IntervalCount < 24));
	if (IntervalCount > 23)
	{
		Result = Props.ID * TimeSlot;	// default to first interval
	}
	else
	{
		if (StartTime > 1439)
		{
			Result = (1439 - CurrentTime + (Props.ID * TimeSlot));	// next day
		}
		else
		{
			Result = StartTime - CurrentTime;
		}
	}
	if (Result < 0) Result = 0;
	if (Result > 1439) Result = 1439;
	return Result;
}

