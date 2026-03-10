
// Bin temperature module
// Wemos D1 mini Pro, ESP 12F    board: LOLIN(Wemos) D1 R2 & mini  or NodeMCU 1.0 (ESP-12E Module)

// user data saved on the sensor:
// 16 bit data
// 11111111  1111  1111
// bin       cable sensor
// 0-255     0-15  0-15

#include <ESP8266WiFi.h>        // Arduino IDE 'Additional Board' http://arduino.esp8266.com/stable/package_esp8266com_index.json
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <EEPROM.h>

#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiUdp.h>
#include <Wire.h>
#include "DS2482.h" 		    // https://github.com/paeaetech/paeae
#include "ESP2SOTA_RC.h"	// modified from https://github.com/pangodream/ESP2SOTA

# define InoDescription "BinTempsModule 09-Mar-2026"
#define InoID 9036  // change to load default values

#define SDApin  4
#define SCLpin  5

#define MaxSensors 200
const uint32_t SampleTime = 3600000;    // update temps every hour

struct ModuleData
{
	byte ID = 0;
	char Name[12] = "Module0";
	uint16_t Port = 1600;
	byte Pin1 = 5;  // D1
	byte Pin2 = 4;  // D2
	byte Pin3 = 14; // D5
	byte Pin4 = 12; // D6
	bool UseDS2482 = false;
	bool StrongPullup = false;
	byte PullupPin = 13;
};

ModuleData MDL;

struct ModuleNetwork
{
	uint16_t Identifier = 7654;
	char SSID[32] = "ssid";
	char Password[32] = "password";
	bool UseWifi = false;
};

ModuleNetwork MDLnetwork;

OneWire OWbus[] = { OneWire(MDL.Pin1),OneWire(MDL.Pin2), OneWire(MDL.Pin3),OneWire(MDL.Pin4) };
byte BusCount = 4;

DS2482 OneWireMaster(0);

struct SensorData
{
	byte ID[8];
	byte BusID;
	byte Temperature[2];	// msb [1], lsb [0]
	byte UserData[2];
};

SensorData Sensors[MaxSensors];

const long pulsewait = 90000;                       // time to wait for a heartbeat before attempting to reconnect (milliseconds)
unsigned long pulsetime = millis() - pulsewait;     // set expired on startup so WiFi connects immediately

unsigned long LoopTime;
WiFiUDP udp;
byte SensorCount = 0;
bool DS2842Connected = false;
ESP8266WebServer server(80);
uint32_t Readtime;
uint8_t MacAddr[6];

void setup()
{
	uint8_t ErrorCount;
	Serial.begin(38400);
	delay(5000);
	Serial.println();
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	EEPROM.begin(256);

	int16_t StoredID;
	EEPROM.get(0, StoredID);
	if (StoredID == InoID)
	{
		Serial.println("Loading stored settings.");
		EEPROM.get(10, MDL);
	}
	else
	{
		Serial.println("Updating stored settings.");
		EEPROM.put(0, InoID);
		EEPROM.put(10, MDL);
		EEPROM.commit();
	}

	EEPROM.get(100, MDLnetwork);
	if (MDLnetwork.Identifier != 7654)
	{
		Serial.println("Network settings not found, using defaults.");
		MDLnetwork.Identifier = 7654;
		strncpy(MDLnetwork.SSID, "ssid", sizeof(MDLnetwork.SSID));
		strncpy(MDLnetwork.Password, "password", sizeof(MDLnetwork.Password));
		MDLnetwork.UseWifi = false;
		EEPROM.put(100, MDLnetwork);
		EEPROM.commit();
	}

	Serial.print("Module name: ");
	Serial.println(MDL.Name);

	Serial.print("Module ID: ");
	Serial.println(MDL.ID);

	// start with station mode off
	WiFi.disconnect(true);

	WiFi.macAddress(MacAddr);
	String AP = MDL.Name;
	AP += "  (";
	AP += WiFi.macAddress();
	AP += ")";
	WiFi.softAP(AP);

	Serial.print("Access Point name: ");
	Serial.println(AP);

	Serial.print("Settings Page IP: ");
	Serial.println(WiFi.softAPIP());

	// web server
	Serial.println();
	Serial.println("Starting Web Server");
	server.on("/", HandleRoot);
	server.on("/page1", HandleTemps);
	server.on("/page2", HandlePage2);
	server.on("/UpdateTemps", DoUpdate);
	server.onNotFound(HandleRoot);
	server.begin();

	// DS2482
	if (MDL.UseDS2482)
	{
		ErrorCount = 0;
		Serial.println("");
		Serial.println("Connecting to DS2482 One-Wire Master...");
		Wire.begin(SDApin, SCLpin);
		OneWireMaster.reset();

		while (!DS2842Connected)
		{
			//configure DS2482 to use active pull-up instead of pull-up resistor 
			DS2842Connected = OneWireMaster.configure(DS2482_CONFIG_APU);
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");
		if (DS2842Connected)
		{
			Serial.println("DS2482 found.");
		}
		else
		{
			Serial.println("DS2482 not found.");
		}
	}
	else
	{
		Serial.println("");
		Serial.println("DS2482 disabled.");
		Serial.println("Using GPIOs for signals.");
	}

	if (MDL.StrongPullup)
	{
		// provide 5V for DS2842 to use for strong pullup
		pinMode(MDL.PullupPin, OUTPUT);
		digitalWrite(MDL.PullupPin, HIGH);
	}

	Readtime = millis() - SampleTime;

	/* INITIALIZE ESP2SOTA LIBRARY */
	ESP2SOTA.begin(&server);
	Serial.println("");
	Serial.println("OTA started.");

	Serial.println("");
	Serial.println("Finished Setup");
	Serial.println("");
}

void loop()
{
	server.handleClient();

	if (MDLnetwork.UseWifi)
	{
		if (WiFi.status() == WL_CONNECTED)
		{
			ReceiveData();
		}

		if (millis() - pulsetime > pulsewait)
		{
			pulsetime = millis();
			if (WiFi.status() != WL_CONNECTED)
			{
				ConnectWifi();
			}
			else
			{
				SendModuleDescription();	// re-announce if WiFi ok but server went quiet
			}
		}
	}

	if (millis() - Readtime > SampleTime)
	{
		Readtime = millis();

		UpdateTemps();

		int Min = SampleTime / 60000;
		Serial.print("");
		Serial.print("Updating in ");
		Serial.print(Min);
		Serial.println(" minutes.");
	}
}

bool GoodCRC(byte Data[], byte Length)
{
	byte ck = CRC(Data, Length - 1, 0);
	bool Result = (ck == Data[Length - 1]);
	return Result;
}

byte CRC(byte Chk[], byte Length, byte Start)
{
	byte Result = 0;
	for (int i = Start; i < Length; i++)
	{
		Result += Chk[i];
	}
	return Result;
}

void SaveData()
{
	EEPROM.put(0, InoID);
	EEPROM.put(10, MDL);
	EEPROM.put(100, MDLnetwork);
	EEPROM.commit();

	delay(3000);

	ESP.restart();
}


void UpdateTemps()
{
	if (DS2842Connected)
	{
		UpdateSensorsMaster();
	}
	else
	{
		UpdateSensors();
	}
}




