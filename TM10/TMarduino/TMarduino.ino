#include <EEPROM.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>

#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <ESP8266mDNS.h>

#define AppName "Temperature Monitor"
#define AppVersion "27-Dec-2020"

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
unsigned int SendToPort = 2388; //port that listens

// sensors
// Wemos D1
OneWire OWbus[3] = { OneWire(12),OneWire(13),OneWire(14) };
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
int TimeSlot = 15;	// time (minutes) allocated to a controlbox to transmit data

bool ReceivedReply = false;
bool UDPstarted = false;

IPAddress SourceIP;
String UpdateURL;
unsigned long UpdateTime;

void setup()
{
	Serial.begin(38400);
	delay(5000);

	Serial.println();
	Serial.print(AppName);
	Serial.print("  :  ");
	Serial.println(AppVersion);
	Serial.println();

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
}

bool WifiConnected()
{
	return ((WiFi.status() == WL_CONNECTED) && (WiFi.RSSI() > -90) && (WiFi.RSSI() != 0));
}

bool CheckWifi(int NumberTries = 1)
{
	if ((millis() - CommTime > 5000) || NumberTries > 1)
	{
		int Count = 0;
		do
		{
			Count++;

			Serial.println();
			Serial.println("Controlbox: " + String(Props.ID));
			Serial.print("Network: ");
			Serial.println(Props.SSID);
			Serial.print("Wifi status: ");
			Serial.println(WiFi.status());
			Serial.print("RSSI: ");
			Serial.println(WiFi.RSSI());
			Serial.print("IP: ");
			Serial.println(WiFi.localIP());
			Serial.print("UDP Destination IP: ");
			Serial.println(ipDestination);
			Serial.println("Update URL: " + UpdateURL);
			Serial.println();

			if (!WifiConnected())
			{
				Serial.print("Connecting to ");
				Serial.println(Props.SSID);

				WiFi.disconnect();
				delay(500);
				WiFi.mode(WIFI_STA);
				WiFi.begin(Props.SSID, Props.Password);
				delay(5000);
				ReconnectCount++;
				ConnectedCount = 0;
				Serial.print("RSSI: ");
				Serial.println(WiFi.RSSI());
				if (ReconnectCount > 10) ESP.restart();
				UDPstarted = false;
			}
			else
			{
				ConnectedCount++;
				ReconnectCount = 0;
			}
			Serial.println("Reconnect count: " + String(ReconnectCount));
			Serial.println("Connected count: " + String(ConnectedCount));
			Serial.println("Minutes connected: " + String(ConnectedCount * 5 / 60));
			Serial.println();
			CommTime = millis();

		} while (Count < NumberTries && !WifiConnected());
	}

	if (!UDPstarted)
	{
		UDPstarted = UDP.begin(ReceiveFromPort);

		// UDP destination
		ipDestination = WiFi.localIP();
		ipDestination[3] = 255;		// change to broadcast
	}

	if (WifiConnected())
	{
		return true;
	}
	else
	{
		return false;
	}
}
