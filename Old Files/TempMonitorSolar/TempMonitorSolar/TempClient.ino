// 06 Apr 2016

// The purpose of this sketch is to report temperatures from DS18B20 sensors and send the data
// to a server on a wifi network.

// Connect multiple DS18B20 temp sensors using pin 0 for data, GND for ground and Vin for power
// to a 'ESP8266 Thing' from Sparkfun. Connect a 4.7K ohm resister between pin 0 data and Vin power.
// See the 'Hookup Guide' at https://learn.sparkfun.com/tutorials/esp8266-thing-hookup-guide

// Use a server on the wifi network to communicate with 'The Thing'. Send and receive packets for 
// communication as described below. A VB6 program and a Winsock control is one example of a server
// that will work.

// The server must send a heartbeat signal within the defined time 'pulsewait' or the sketch will assume
// the connection is lost and attempt to reconnect. Packets send and receive ROM code adresses as 
// strings in the form 'xx xx xx xx xx xx xx xx'. The 'xx' is a hex value of each of the 8 bytes
// of the address.

// The DS18B20 uses a 8 byte ROM code. The ESP8266 uses a 6 byte Mac address.

// packet description:
// start,packet type,break,data,break,sensor Rom Code,break
// packet types:                                                  examples:
// 0  heartbeat to signal still connected                         ^0|||       heartbeat
// 1  command sensors to report either 0 or a specific board      ^1||0|      all sensors report
// 2  set userdata                                                ^2|NewValue|RomCode|     set userdata for sensor
// 3  get userdata                                                ^3||RomCode|     get userdata for sensor

// user data saved on the sensor:
// 16 bit data
// 11111111  1111  1111
// bin       cable sensor
// 0-255     0-15  0-15

void ConnectServerNetwork()
{
  int count = 0;
   while (WiFi.status() != WL_CONNECTED)
  {
    count = count + 1;
    if (count > RetryMax)return;  // return to loop to check for mode switch change
    WiFi.begin(clientSSID, clientPassword);
    delay(3000);
    WiFi.mode(WIFI_STA);
    apStarted=false;
  } 
}
void ConnectServer()
{
  int count = 0;
  boolean ServerOnLine = ServerIsConnected();
  while (!ServerOnLine)
  {
    count = count + 1;
    if (count > RetryMax)
    {
      WiFi.disconnect();
      return;
    }
    Serial.print("0");
    if (count < RetryMax / 2)
    {
      // try saved server IP
      ServerOnLine = client.connect(ServerIP, portnum);
    }
    else
    {
      // try default server IP
      ServerOnLine = client.connect(DefaultServerIP,portnum);
    }    
    if (ServerOnLine)
    {
      pulsetime = millis();
      PulseInit = false;
    }
    delay(3000);
  }
}

void readserver()
{
  int Count = 0;
  while (client.available())
  {
    char c = client.read();
    Count = Count + 1;
    if (Count > maxread) // max characters
    {
      return;
    }
    else
    {
      packetin = packetin + c;
    }
  }
}

void checkchunks()
{
    while (getchunks())  //break message up into parts, keep getting chunks from packet
    {
      processchunks();  //take action based on message
      delay(100);
    }
    packetin="";  //erase any junk
}

boolean getchunks()
{
  int St=packetin.indexOf("^");
  if(St == -1 )
  {
    // ignore message, no start character
    return false;
  }
  int D1=packetin.indexOf("|",St + 1);
  if (D1 == -1)
  {
    return false;
  }
  int D2=packetin.indexOf("|",D1 + 1);
  if (D2 == -1)
  {
    // no second delimiter
    return false;
  }
  int D3 = packetin.indexOf("|",D2 + 1);
  if (D3 == -1)
  {
    // no third delimiter
    return false;
  }
  P1 = packetin.substring(St + 1, D1).toInt();
  P2 = packetin.substring(D1 + 1, D2);
  P3 = packetin.substring(D2 + 1, D3);
  packetin.remove(0,D3);  // remove processed part
  return true;
}

void processchunks()
{
    switch (P1)
    {
      case 0:
        // heartbeat, reset time
        pulsetime = millis();
        break;
      case 1:
        // get temperatures
        sendtempsBus1();
        sendtempsBus2();
        break;
      case 2:
        //set user data
        setuserdataBus1();
        setuserdataBus2();
        break;
      case 3:
        //get user data
        getuserdataBus1();
        getuserdataBus2();
        break;
    }
}

void sendpacket(int id,String data,String Rom)
{
  if (client.connected())
  {
    data = "^" + String(id) + "|" + data + "|" + Rom + "|";
    client.print(data);
  }
}

String addressTostring(byte addr[8])    //converts 8 byte address to hex and places a space between bytes
{
  String ans="";
  for( byte i=0; i < 8; i++)
  {
    delay(100);
    ans=ans + DecToHex(addr[i]);
    ans=ans + " ";
  }
  return ans;
}

void stringToaddress(String ad)   //converts 8 hex spaced address string to 8 byte decimal
{
  int bcount = 0;
  int len = ad.length() + 1;
  char prt[len];
  ad.toCharArray(prt,len);      // convert string to character array
  String v="";
  for (int p=0; p < len; p++)   // go through array to find hex address parts between spaces
  {
    if (prt[p] == ' ')          // check for space delimiter
    {
      RomCode[bcount] = hexToDec(v); //convert string hex address part to byte
      v="";
      bcount = bcount + 1;
      if (bcount ==8)
      {
        return; //done
      }
    }
    else
    {
      v = v + String(prt[p]); //build string hex address part
    }
  }
  RomCode[bcount] = hexToDec(v);  // needed if there is no last space
}

byte hexToDec(String hexString)
{  
  byte decValue = 0;
  int nextInt;  
  for (int i = 0; i < hexString.length(); i++) 
  {   // only allow 2 character hex address parts    
    nextInt = int(hexString.charAt(i));
    if (nextInt >= 48 && nextInt <= 57) nextInt = map(nextInt, 48, 57, 0, 9);
    if (nextInt >= 65 && nextInt <= 70) nextInt = map(nextInt, 65, 70, 10, 15);
    if (nextInt >= 97 && nextInt <= 102) nextInt = map(nextInt, 97, 102, 10, 15);
    nextInt = constrain(nextInt, 0, 15);   
    decValue = (decValue * 16) + nextInt;
  }  
  return decValue;
}

String DecToHex(byte dec)
{
  String ans = String(dec,HEX);
  ans.toUpperCase();
  if (ans.length() == 1)
  { 
    ans = "0" + ans;
  }
  return ans;
}

