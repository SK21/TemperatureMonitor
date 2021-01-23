// Wemos D1 mini Pro,  board: LOLIN(Wemos) D1 R2 & mini

#include <EEPROM.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>

#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <ESP8266mDNS.h>

//bool ForceWebPage = true;
bool ForceWebPage = false;

#define UseDS2482	1	// 0 don't use, 1 use

#define SDApin  12
#define SCLpin  14

int BusCount = 3;

#define AppName "Temperature Monitor"
#define AppVersion "23-Jan-2021"

#if UseDS2482

#include <Wire.h>
#include<DS2482.h>		// https://github.com/paeaetech/paeae
DS2482 OWbus(0);

#else

// Wemos D1 mini Pro
OneWire OWbus[] = { OneWire(12),OneWire(13),OneWire(14) };

#endif

ESP8266WebServer server(80);

extern "C"
{
#include "user_interface.h"		// for RTC memory access
}
#define RTCMEMORYSTART 65

// wifi
#define CommLength 17
byte OutBuffer[CommLength];	 // Array to send data back
byte InBuffer[255];	 //buffer to hold incoming packet
WiFiUDP UDP;
unsigned int ReceiveFromPort = 8120;      // local port to listen on

//sending back to where and which port
IPAddress ipDestination;
unsigned int SendToPort = 1688; //port that listens

byte CurrentSensorAddress[8];

// commands:
// 1 all sensor report
// 2 specific sensor report
// 3 set sensor userdata
byte CommandByte;

unsigned long CommTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;

int UserData;

byte SensorCount = 0;
bool SendEnabled = false; // if last PGN received was for this node send is enabled

struct SensorData
{
	byte ID[8];
	byte BusID;
	byte Temperature[2];	// msb [1], lsb [0]
	byte UserData[2];
};

SensorData Sensors[255];

struct ControlBoxData
{
	byte ID;	// unique to each control box
	char SSID[32];
	char Password[32];
	byte UseSleep;
	int SleepInterval;
	byte ControlBoxCount;
	byte Check;	// validity check
};

ControlBoxData BoxData;

struct WakeTimeData
{
	int MinutesRemaining;	// needs to be 4 bytes each for RTC memory
	int Check;
};

WakeTimeData WT;

int CurrentTime = 0;	// 0-1439 minutes (24 hrs)
unsigned long FlashTime;
bool FlashState;

unsigned int PGN;
int TimeSlot = 5;	// time (minutes) allocated to a controlbox to transmit data
bool ReceivedReply = false;
unsigned long UpdateTime;

byte LineUDS;
byte LineASR;
byte LineSensors;

unsigned long DiagTime;

bool SendDiag = false;
bool Restart = false;

void setup()
{
	Serial.begin(38400);
	delay(5000);

	Serial.println();
	Serial.print(AppName);
	Serial.print("  :  ");
	Serial.println(AppVersion);
	Serial.println();
	if (UseDS2482)
	{
		Serial.println("Using DS2482.");
	}
	else
	{
		Serial.println("Not using DS2482.");
	}
	Serial.println();

	pinMode(D0, WAKEUP_PULLUP);

	NetConnect();

	StartOTA();

	pinMode(LED_BUILTIN, OUTPUT);
	digitalWrite(LED_BUILTIN, HIGH);	// LED off

#if UseDS2482

	Serial.println();
	Wire.begin(SDApin, SCLpin);
	OWbus.reset();

	//configure DS2482 to use active pull-up instead of pull-up resistor 
	//configure returns 0 if it cannot find DS2482 connected 
	if (OWbus.configure(DS2482_CONFIG_APU))
	{
		Serial.println("DS2482 found.");
	}
	else
	{
		Serial.println("DS2482 not found.");
	}

#endif

	UpdateSensors();	// get an initial count of connected sensors
}

void loop()
{
	ArduinoOTA.handle();
	if (BoxData.UseSleep)
	{
		DoSleepMode();
	}
	else
	{
		DoClientMode();
	}

	if (SendDiag) SendDiagnostics();
	if (Restart) ESP.restart();
}

