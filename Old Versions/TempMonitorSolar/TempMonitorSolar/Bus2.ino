void  sendtempsBus2()
{
  if(P3=="")
  {
    // get all sensors
    // Group 1
    DTbus2.requestTemperatures();
    delay(1000);
    OWbus2.reset_search();
    while (OWbus2.search(sensoraddress))
    {
      packetout = DTbus2.getTempC(sensoraddress);
      String Rom = addressTostring(sensoraddress);
      sendpacket(1,packetout,Rom);
      delay(1000);
    }
  }
  else
  {
    // get single sensor with Mac address P3
    stringToaddress(P3);
//    sensors.requestTemperaturesByAddress(RomCode);   // crashes
    DTbus2.requestTemperatures();
    delay(1000);
    packetout = DTbus2.getTempC(RomCode);
    sendpacket(1,packetout,P3);
  }
}

void PushTempsBus2()
{
    // get all sensors
  DTbus2.requestTemperatures();
  delay(1000);
  OWbus2.reset_search();
  while (OWbus2.search(sensoraddress))
  {
    packetout = DTbus2.getTempC(sensoraddress);
    String Rom = addressTostring(sensoraddress);
    sendpacket(1,packetout,Rom);
    delay(1000);
  } 
  delay(1000); 
}

void handleTempsBus2()
{
   // get all sensors
  DTbus2.requestTemperatures();
  delay(1000);
  OWbus2.reset_search();
  while (OWbus2.search(sensoraddress))
  {
    packetout = DTbus2.getTempC(sensoraddress);
    getIDsBus2(sensoraddress);
    String Rom = addressTostring(sensoraddress);
    server.sendContent(
      String() + "<tr><td>Sensor ID  [" + Rom + "]"
      "  Bin " + binID + ","
      "  Cable " + cableID + ","
      "  Sensor " + sensorID + ","
      "  (" + packetout + ")</td></tr><br><br>"
      );
    delay(1000);
  }
}

void getIDsBus2(byte SenCode[8])
{
  word ud = DTbus2.getUserData(SenCode); // get user data
  word tmp = ud;
  tmp = tmp >> 8;
  binID = tmp;
  tmp = ud;
  tmp = tmp & 240;
  cableID = tmp >>4;
  sensorID = ud & 15;
}

void setuserdataBus2()
{
  stringToaddress(P3);
  int userdata = P2.toInt();
  DTbus2.setUserData(RomCode,userdata);        // save new data 
  DTbus2.setResolution(RomCode,resolution);    // make sure resolution is set on sensor
}

void getuserdataBus2()
{
  stringToaddress(P3);
  packetout = String(DTbus2.getUserData(RomCode)); // get user data
  sendpacket(3,packetout,P3);
}
