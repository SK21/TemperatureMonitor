// 31 Oct 2018
// client only version of Temperature Monitor

// The purpose of this sketch is to report temperatures from DS18B20 sensors and send the data
// to a server on a wifi network.

// Connect multiple DS18B20 temp sensors using pin 0 for data, GND for ground and Vin for power
// to a 'ESP8266 Thing' from Sparkfun. Connect a 4.7K (2.2K for longer runs) ohm resister between pin 0 data and Vin power.
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

#include <ESP8266WiFi.h>        // Arduino IDE 'Additional Board' http://arduino.esp8266.com/stable/package_esp8266com_index.json
#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <DallasTemperature.h>  // https://github.com/milesburton/Arduino-Temperature-Control-Library
#include <WiFiClient.h> 


// **************  user setup *******************
char clientSSID[32] = "ssid";     //network name
char clientPassword[32] = "password"; //network password
char DefaultServerIP[32] = "192.168.xx.xx";	// server IP
const int SignalPin1 = 4;  // data signal pin for group 1, brown cat5
const int SignalPin2 = 5; // data signal pin for group 2,  green cat5
const int SignalPinP = 0; // data signal pin for parasitic mode, 2.2K ohm between 3.3v and black wire, red ground
const byte resolution = 9;       // 9, 10, 11, or 12 bits, corresponding to increments of 0.5°C, 0.25°C, 0.125°C, and 0.0625°C, respectively
// ************** end user setup ****************

// sensors
OneWire OWbus1(SignalPin1); // one wire object for group 1
OneWire OWbus2(SignalPin2);
OneWire OWbusP(SignalPinP);
DallasTemperature DTbus1(&OWbus1);
DallasTemperature DTbus2(&OWbus2);
DeviceAddress sensoraddress;
int binID;
int cableID;
int sensorID;
byte RomCode[8];    // for sensor address
float SensorTemp;

WiFiClient client;
const long portnum = 1600;
boolean serverconnected = false;
const long pulsewait = 60000;   //time to wait for a pulse before attempting to reconnect (milliseconds)
unsigned long pulsetime = millis() - pulsewait;  // set to connect to server on startup

int maxread = 50;   // maximum # of characters to read from server
String packetin = "";
String packetout = "";
int P1 = 0;       // packet type
String P2 = "";   // packet data
String P3 = "";   // packet sensor address

void setup()
{
  Serial.begin(57600);
  Serial.println();
  DTbus1.begin();
  DTbus2.begin();
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);
}

void loop()
{
    if (!serverconnected)checkServerNetwork();
    checkserver();    //check server connected
    readserver();     //check for message from server
    checkchunks();    //break up and process message
	FlashPin();
}

void FlashPin()
{
	digitalWrite(LED_BUILTIN, LOW);  // LED on
	delay(100);
	digitalWrite(LED_BUILTIN, HIGH); // LED off
	delay(100);
}

void checkServerNetwork()
{
	while (WiFi.status() != WL_CONNECTED)
	{
		WiFi.begin(clientSSID, clientPassword);
		delay(5000);
		WiFi.mode(WIFI_STA);
	}
}

void checkserver()
{
	int count = 0;
	serverconnected = (pulsewait > (millis() - pulsetime));
	while (!serverconnected)
	{
		count = count + 1;
		if (count > 10)
		{
			WiFi.disconnect();
			return;
		}
		serverconnected = client.connect(DefaultServerIP, portnum);
		if (serverconnected)
		{
			pulsetime = millis();
		}
		delay(1000);
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
	packetin = "";  //erase any junk
}

boolean getchunks()
{
	int St = packetin.indexOf("^");
	if (St == -1)
	{
		// ignore message, no start character
		return false;
	}
	int D1 = packetin.indexOf("|", St + 1);
	if (D1 == -1)
	{
		return false;
	}
	int D2 = packetin.indexOf("|", D1 + 1);
	if (D2 == -1)
	{
		// no second delimiter
		return false;
	}
	int D3 = packetin.indexOf("|", D2 + 1);
	if (D3 == -1)
	{
		// no third delimiter
		return false;
	}
	P1 = packetin.substring(St + 1, D1).toInt();
	P2 = packetin.substring(D1 + 1, D2);
	P3 = packetin.substring(D2 + 1, D3);
	packetin.remove(0, D3);  // remove processed part
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
		sendtempsBusP();
		break;
	case 2:
		//set user data
		setuserdataBus1();
		setuserdataBus2();
		setuserdataBusP();
		break;
	case 3:
		//get user data
		getuserdataBus1();
		getuserdataBus2();
		getuserdataBusP();
		break;
	}
}

void sendpacket(int id, String data, String Rom)
{
	data = "^" + String(id) + "|" + data + "|" + Rom + "|";
	client.print(data);
}

String addressTostring(byte addr[8])    //converts 8 byte address to hex and places a space between bytes
{
	String ans = "";
	for (byte i = 0; i < 8; i++)
	{
		delay(100);
		ans = ans + DecToHex(addr[i]);
		ans = ans + " ";
	}
	return ans;
}

void stringToaddress(String ad)   //converts 8 hex spaced address string to 8 byte decimal
{
	int bcount = 0;
	int len = ad.length() + 1;
	char prt[len];
	ad.toCharArray(prt, len);      // convert string to character array
	String v = "";
	for (int p = 0; p < len; p++)   // go through array to find hex address parts between spaces
	{
		if (prt[p] == ' ')          // check for space delimiter
		{
			RomCode[bcount] = hexToDec(v); //convert string hex address part to byte
			v = "";
			bcount = bcount + 1;
			if (bcount == 8)
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
	String ans = String(dec, HEX);
	ans.toUpperCase();
	if (ans.length() == 1)
	{
		ans = "0" + ans;
	}
	return ans;
}

