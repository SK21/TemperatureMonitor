void  sendtempsBus1()
{
  if(P3=="")
  {
    // get all sensors
    // Group 1
    DTbus1.requestTemperatures();
    delay(1000);
    OWbus1.reset_search();
    while (OWbus1.search(sensoraddress))
    {
      packetout = DTbus1.getTempC(sensoraddress);
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
    DTbus1.requestTemperatures();
    delay(1000);
    packetout = DTbus1.getTempC(RomCode);
    sendpacket(1,packetout,P3);
  }
}

void PushTempsBus1()
{
    // get all sensors
  DTbus1.requestTemperatures();
  delay(1000);
  OWbus1.reset_search();
  while (OWbus1.search(sensoraddress))
  {
    packetout = DTbus1.getTempC(sensoraddress);
    String Rom = addressTostring(sensoraddress);
    sendpacket(1,packetout,Rom);
    delay(1000);
  } 
  delay(1000); 
}

void handleTempsBus1()
{
   // get all sensors
  DTbus1.requestTemperatures();
  delay(1000);
  OWbus1.reset_search();
  while (OWbus1.search(sensoraddress))
  {
    packetout = DTbus1.getTempC(sensoraddress);
    getIDsBus1(sensoraddress);
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

void getIDsBus1(byte SenCode[8])
{
  word ud = DTbus1.getUserData(SenCode); // get user data
  word tmp = ud;
  tmp = tmp >> 8;
  binID = tmp;
  tmp = ud;
  tmp = tmp & 240;
  cableID = tmp >>4;
  sensorID = ud & 15;
}

void setuserdataBus1()
{
  stringToaddress(P3);
  int userdata = P2.toInt();
  DTbus1.setUserData(RomCode,userdata);        // save new data 
  DTbus1.setResolution(RomCode,resolution);    // make sure resolution is set on sensor
}

void getuserdataBus1()
{
  stringToaddress(P3);
  packetout = String(DTbus1.getUserData(RomCode)); // get user data
  sendpacket(3,packetout,P3);
}
