// The WiFi radio is switched off for all but the last wakeup to save power
// Config data is stored in EEPROM starting at byte 'StartAddress'
// byte 1 (StartAddress + 0) is a flag used to check sleep cycle, 100 means in sleep cycle
// byte 2 (StartAddress + 1) is the sleep counter
// byte 3 (StartAddress + 2) is the radio status, 100 means the radio is on

const int StartAddress = 200;
void CheckSleepInterval()
{
  byte Edata[2];
  EEPROM.begin(512);
  EEPROM.get(StartAddress + 0,Edata[0]);  
  EEPROM.get(StartAddress + 1,Edata[1]); 
  if (Edata[0] == 100)
  {
    Edata[1] += 1;
    if (Edata[1] > IntervalSleeps) Edata[1] = 0;
  }
  else
  {
    // first run, initialize sleep cycle
    Edata[0] = 100;
    Edata[1] = 0;
    EEPROM.put(StartAddress + 0, Edata[0]);
  }
  EEPROM.put(StartAddress + 1, Edata[1]);
  EEPROM.commit();
  EEPROM.end();
  SendTime = (millis() - PushStart) * 1000; //correct for time taken to send data
  if (Edata[1] == IntervalSleeps)
  {
    DoSleep(SleepMicroseconds - SendTime,true);
  }
  else if (Edata[1] > 0)
  {
    DoSleep(SleepMicroseconds - SendTime,false);
  }
  else
  {
    // start of interval, return for work cycle
  }
}

void RadioOn()
{
  // make sure radio is on
  byte Status;
  EEPROM.begin(512);
  EEPROM.get(StartAddress + 2,Status);
  EEPROM.put(StartAddress + 0,0); // reset sleep flag
  EEPROM.commit();
  EEPROM.end();
  if (Status != 100) DoSleep(1,true); // turn on radio if not already on
}

void DoSleep(uint32_t SleepTime, boolean RFon)
{
  if (DTRconnectedXPD)
  { 
    if (SleepTime > SleepMax) SleepTime = SleepMax;
    if (RFon)
    {
      EEPROM.begin(512);
      EEPROM.put(StartAddress + 2,100); // record status as radio on
      EEPROM.commit();
      EEPROM.end();
      client.stop();
      server.stop(); 
      WiFi.disconnect();
      delay(100);
      ESP.deepSleep(SleepTime, WAKE_RF_DEFAULT);
//      Serial.println(); 
//      Serial.println("Sleep time (s) = " + String(SleepTime/1000000));
//      delay(2000);
//      ESP.restart();
    }
    else
    {
      EEPROM.begin(512);
      EEPROM.put(StartAddress + 2,0); // record status as radio off
      EEPROM.commit();
      EEPROM.end();
      client.stop();
      server.stop(); 
      WiFi.disconnect();
      delay(100);
      ESP.deepSleep(SleepTime, WAKE_RF_DISABLED);
//      Serial.println(); 
//      Serial.println("Sleep time (s) = " + String(SleepTime/1000000));
//      delay(2000);
      ESP.restart();
    }
  }
}

void ResetSleep()
  {
    // Reset EEPROM sleep cycle flag
    byte Edata[2];
    EEPROM.begin(512);
    Edata[0] = 0;
    EEPROM.put(StartAddress + 0, Edata[0]);    
    EEPROM.commit();
    EEPROM.end();
   }
  
