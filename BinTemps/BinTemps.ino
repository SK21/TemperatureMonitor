
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
#include <ArduinoOTA.h>
#include <EEPROM.h>

#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiClient.h> 
#include <Wire.h>
#include "DS2482.h" 		    // https://github.com/paeaetech/paeae

# define InoDescription "BinTemps 08-Mar-2026"
#define InoID 8036  // change to load default values

#define SDApin  4
#define SCLpin  5

#define MaxSensors 200
const uint32_t SampleTime = 3600000;    // update temps every hour

struct ModuleData
{
	byte ID = 0;
	char Name[12] = "Module0";
	char SSID[32] = "ssid";
	char Password[32] = "password";
	IPAddress ServerIP = IPAddress(192, 168, 1, 1);
	uint16_t Port = 1600;
	byte Pin1 = 5;  // D1
	byte Pin2 = 4;  // D2
	byte Pin3 = 14; // D5
	byte Pin4 = 12; // D6
	bool UseDS2482 = false;
	bool UseWifi = false;
	bool StrongPullup = false;
	byte PullupPin = 13;
};

ModuleData MDL;

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

const long pulsewait = 60000;                       // time to wait for a pulse before attempting to reconnect (milliseconds)
unsigned long pulsetime = millis() - pulsewait;     // set to connect to server on startup
bool ServerConnected;
const long ServerConnectInterval = 180000;                 // time to wait to reconnect to temperature server
unsigned long LastServerConnectTime = millis() - ServerConnectInterval;

unsigned long LoopTime;
WiFiClient client;
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

	EEPROM.begin(512);
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

	Serial.print("Module name: ");
	Serial.println(MDL.Name);

	StartOTA();

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

	// web server
	Serial.println();
	Serial.println("Starting Web Server");
	server.on("/", HandleRoot);
	server.on("/page1", HandleTemps);
	server.on("/page2", HandlePage2);
	server.on("/update", DoUpdate);
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

	Serial.println("");
	Serial.println("Finished Setup");
	Serial.println("");
}

void loop()
{
	ArduinoOTA.handle();
	server.handleClient();

	if ((MDL.UseWifi) && (WiFi.status() == WL_CONNECTED))
	{
		CheckTempServer();    //check server connected
		ReceiveData();     //check for message from server
	}

	if (millis() - Readtime > SampleTime)
	{
		Readtime = millis();

		if (MDL.UseWifi && (WiFi.status() != WL_CONNECTED))
		{
			ConnectWifi();
			CheckTempServer();
		}

		UpdateTmps();

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
	EEPROM.begin(512);
	EEPROM.put(0, InoID);
	EEPROM.put(10, MDL);
	EEPROM.commit();

	delay(3000);

	ESP.restart();
}



void UpdateTmps()
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

String IPtoString(IPAddress addr)
{
	String ans = String(addr[0]) + "." + String(addr[1]) + "." + String(addr[2]) + "." + String(addr[3]);
	return ans;
}



