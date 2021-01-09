// for use with Nano 33, DS2482 1-Wire Master and DS18B20 temperature sensors

#include <WiFiNINA.h>
#include <OneWire.h>
#include <Wire.h>
#include<DS2482.h>		// https://github.com/paeaetech/paeae
#include <ArduinoOTA.h>	// https://github.com/jandrassy/ArduinoOTA

// user settings *****************************
#define WifiSSID ""
#define WifiPassword ""
#define ControlBoxID 5	// range 0-255, unique to each control box
// *******************************************

int ConnectionStatus = WL_IDLE_STATUS;
char ssid[] = WifiSSID;        // your network SSID 
char pass[] = WifiPassword;    // your network password 

byte InBuffer[150];	 //buffer to hold incoming packet
byte OutBuffer[16];	 // Array to send data back 

WiFiUDP UDPin;
WiFiUDP UDPout;

// UDP
unsigned int ReceiveFromPort = 8120;      // local port to listen on
unsigned int SendFromPort = 1480;

//sending back to where and which port
static byte ipDestination[] = { 192, 168, 2, 255 };
unsigned int SendToPort = 1688; //port that listens

unsigned long CommTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;

// commands:
// 1 all sensor report
// 2 specific sensor report
// 3 set sensor userdata
byte CommandByte;	

int UserData;
float SensorTemp;

unsigned long LEDtime;
bool LEDstate;

DS2482 OWbus(0);

struct SensorData
{
	byte ID[8];
	byte Temperature[2];
	byte UserData[2];
};

SensorData Sensors[255];

bool SensorsCounted;
byte SensorCount;
float Result;

byte dsScratchPadMem[9];
long RawTemp = 0;
byte data[8];

unsigned int PGN;
bool SendEnabled;
byte SensorAddress[8];

int status = WL_IDLE_STATUS;
byte LineUDS;
byte LineASR;
unsigned long DiagTime;

void setup()
{
	//set up communication
	Serial.begin(38400);
	pinMode(LED_BUILTIN, OUTPUT);

	delay(5000);
	Serial.println();
	Serial.println("Temperature Monitor Bridge  :  08-Jan-2021");
	Serial.println();

	// check for the WiFi module:
	if (WiFi.status() == WL_NO_MODULE)
	{
		Serial.println("Communication with WiFi module failed!");
		// don't continue
		while (true);
	}

	String fv = WiFi.firmwareVersion();
	Serial.println("Wifi firmware version: " + fv);

	UDPin.begin(ReceiveFromPort);
	UDPout.begin(SendFromPort);
	delay(1000);

	Serial.println();
	Wire.begin();
	OWbus.reset();

	////configure DS2482 to use active pull-up instead of pull-up resistor 
	////configure returns 0 if it cannot find DS2482 connected 
	if (OWbus.configure(DS2482_CONFIG_APU))
	{
		Serial.println("DS2482 found.");
	}
	else
	{
		Serial.println("DS2482 not found.");
	}
	Serial.println();

	// attempt to connect to Wifi network:
	while (status != WL_CONNECTED)
	{
		Serial.print("Attempting to connect to SSID: ");
		Serial.println(ssid);
		status = WiFi.begin(ssid, pass);
	}
	Serial.println("Connected.");
	Serial.println();

	// start the WiFi OTA library with internal (flash) based storage
	ArduinoOTA.begin(WiFi.localIP(), "Arduino", "password", InternalStorage);
	Serial.println("OTA Ready.");
}

void loop()
{
	ArduinoOTA.poll();

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

 //SendDiagnostics();
}

void FlashLED()
{
	if (millis() - LEDtime > 1000)
	{
		LEDstate = !LEDstate;
		digitalWrite(LED_BUILTIN, LEDstate);
		LEDtime = millis();
	}
}
