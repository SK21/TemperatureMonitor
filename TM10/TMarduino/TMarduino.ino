#include <EEPROM.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>

#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <ESP8266mDNS.h>

#define AppName "Temperature Monitor"
#define AppVersion "10-Jan-2021"

//bool UseWebPage = true;
bool UseWebPage = false;

ESP8266WebServer server(80);

extern "C"
{
#include "user_interface.h"		// for RTC memory access
}
#define RTCMEMORYSTART 65

// wifi
byte InBuffer[255];	 //buffer to hold incoming packet
byte OutBuffer[16];	 // Array to send data back
WiFiUDP UDP;
unsigned int ReceiveFromPort = 8120;      // local port to listen on

//sending back to where and which port
IPAddress ipDestination;
unsigned int SendToPort = 1688; //port that listens

// sensors
// Wemos D1
// board: Wemos D1 R2 & mini
OneWire OWbus[] = { OneWire(12),OneWire(13),OneWire(14) };
int BusCount = 3;

 //ESP-01
//OneWire OWbus[2] = { OneWire(0),OneWire(2) };
//int BusCount = 2;

byte SensorAddress[8];

// commands:
// 1 all sensor report
// 2 specific sensor report
// 3 set sensor userdata
byte CommandByte;

unsigned long CommTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;

byte data[8];
int UserData;
float SensorTemp;

long RawTemp = 0;
float Result = 0;
byte dsScratchPadMem[9];

byte SensorCount = 0;
bool SensorsCounted = false;
bool SendEnabled = false; // if last PGN received was for this node send is enabled

struct SensorData
{
	byte ID[8];
	byte BusID;
	byte Temperature[2];
	byte UserData[2];
};

SensorData Sensors[255];

unsigned long FlashTime;
bool FlashState;
unsigned int PGN;

struct Properties
{
	byte ID;	// unique to each control box
	char SSID[32];
	char Password[32];
	byte UseSleep;
	int SleepInterval;
	byte ControlBoxCount;
	byte Check;	// validity check
};

Properties Props;

int CurrentTime = 0;	// 0-1439 minutes (24 hrs)

struct WakeTimeData
{
	int MinutesRemaining;	// needs to be 4 bytes each
	int Check;
};

WakeTimeData WT;

unsigned long SendTime;
int TimeSlot = 1;	// time (minutes) allocated to a controlbox to transmit data

bool ReceivedReply = false;
bool UDPstarted = false;

unsigned long UpdateTime;

byte LineUDS;
byte LineASR;
byte LineSensors;

unsigned long DiagTime;

void setup()
{
	Serial.begin(38400);
	delay(5000);

	Serial.println();
	Serial.print(AppName);
	Serial.print("  :  ");
	Serial.println(AppVersion);
	Serial.println();

	pinMode(D0, WAKEUP_PULLUP);

	NetConnect();

	StartOTA();

	pinMode(LED_BUILTIN, OUTPUT);
	digitalWrite(LED_BUILTIN, HIGH);	// LED off
}

void loop()
{
	ArduinoOTA.handle();
	if (Props.UseSleep)
	{
		DoSleepMode();
	}
	else
	{
		DoClientMode();
	}

	//SendDiagnostics();
}
