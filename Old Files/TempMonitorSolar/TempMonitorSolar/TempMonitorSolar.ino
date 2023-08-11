#include <ESP8266WiFi.h>        // Arduino IDE 'Additional Board' http://arduino.esp8266.com/stable/package_esp8266com_index.json
#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <DallasTemperature.h>  // https://github.com/milesburton/Arduino-Temperature-Control-Library

#include <WiFiClient.h> 
#include <ESP8266WebServer.h>
#include <DNSServer.h>
#include <ESP8266mDNS.h>
#include <EEPROM.h>


//   11 Apr 2017


// customize settings
const char *apPassword = "12345678";  // access point password
const char *myHostname = "TM";  // for access point
char DefaultServerIP[32] = "192.168.2.14";  // in client mode this is the IP of the PC server
const int IntervalSleeps = 12;   // # of sleeps per interval, example: 12 IntervalSleeps X 60 SleepMinutes = 12 hour interval
unsigned long SleepMinutes = 60;  // minutes of each sleep, max 60
const int ClientPin = 13;  // used to go into client mode
const int PushPin = 12; // used to go into push mode
const int SignalPin1 = 14;  // data signal pin for group 1
const int SignalPin2 = 0; // data signal pin for group 2
const boolean DTRconnectedXPD = true; //sleep will only be enabled if jumper connected  
const int LEDpin = 5; // for debugging

// access point
boolean apStarted = false;  
const byte DNS_PORT = 53;
DNSServer dnsServer;
ESP8266WebServer server(80);
IPAddress apIP(192, 168, 4, 1);
IPAddress netMsk(255, 255, 255, 0);

// client mode
WiFiClient client;
const long portnum = 1600;
char clientSSID[32] = "";     //saved client network
char clientPassword[32] = ""; //saved client network password
char ServerIP[32] = "";       // saved ip of remote server we will connect to
unsigned long pulsetime = 0;
const long pulsewait = 60000;   //time to wait for a pulse before attempting to reconnect (milliseconds)
bool PulseInit = true;  //used when sketch is first run to prevent false positive connection status
int maxread = 50;   // maximum # of characters to read from server
String packetin = "";
String packetout = "";
int P1 = 0;       // packet type
String P2 = "";   // packet data
String P3 = "";   // packet sensor address

// push mode
const int PushCountMax = 5; // # of times to try to connect to server before going to sleep
int PushCount = 0;
unsigned long SleepMicroseconds = SleepMinutes * 60 * 1000000; //  minutes * 60 seconds * microseconds/second = sleep time in microseconds
unsigned long SendTime = 0;  // time taken to send data
unsigned long PushStart = 0;    // time push mode started
unsigned long SleepMax = 3600000000;

// sensors
const int resolution = 9;       // 9, 10, 11, or 12 bits, corresponding to increments of 0.5°C, 0.25°C, 0.125°C, and 0.0625°C, respectively
OneWire OWbus1(SignalPin1); // one wire object for group 1
OneWire OWbus2(SignalPin2);
DallasTemperature DTbus1(&OWbus1);
DallasTemperature DTbus2(&OWbus2);
DeviceAddress sensoraddress;
int binID;
int cableID;
int sensorID;
byte RomCode[8];    // for sensor address

// other
const int RetryMax = 10; // used for connecting to network and server
int ConMode=4;    //connect mode, 0 = access point, 1 = client, 2 = push,  set to 4 for initilization
int NewConMode=0; // new mode based on current settings

void setup()
{
  Serial.begin(57600);
  Serial.println();
  DTbus1.begin(); 
  DTbus2.begin();
  loadCredentials();  // load from memory
  pinMode(ClientPin,INPUT_PULLUP);
  pinMode(PushPin,INPUT_PULLUP);
  pinMode(LEDpin, OUTPUT);
  PushStart = millis();
}

void loop()
{
//  FlashPin();
  CheckMode();
  if (ConMode == 0)
  {
    // access point
    Serial.print(":");
    if (apStarted)
    {
      dnsServer.processNextRequest(); //DNS
      server.handleClient();  //HTTP      
    }
    else
    {
      StartAP();
    }
   }
  else if (ConMode == 1)
  {
    // client
    Serial.print("+");
    if (WiFi.status() == WL_CONNECTED)
    {
      if (ServerIsConnected())
      {
        readserver();     //check for message from server
        checkchunks();    //break up and process message
      }
      else
      {
        ConnectServer();      
      }
    }
    else
    {
      ConnectServerNetwork();
    }
  }
  else
  {
    // Push mode
    Serial.print("*");
    if (PushCount == 0) CheckSleepInterval(); // only check interval at start of work cycle
    PushCount += 1;
    if (WiFi.status() == WL_CONNECTED)
    {
      if (ServerIsConnected())
      {
        PushTempsBus1();
        PushTempsBus2();
        PushCount = 0;  // to exit loop
      }
      else
      {
        ConnectServer();      
      }
    }
    else
    {
      ConnectServerNetwork();
    }
    if (PushCount > PushCountMax) PushCount = 0;
  }
}

