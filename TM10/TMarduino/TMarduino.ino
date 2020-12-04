#include <WiFiManager.h>		// https://github.com/tzapu/WiFiManager
#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiUdp.h>

// user settings *****************************
byte ControlBoxID = 1;		// unique to each control box
// *******************************************

#define APP_Name "Temperature Monitor"
#define App_Version "29-Nov-2020"

// wifi
byte ConnectionStatus = WL_IDLE_STATUS;
byte InBuffer[255];	 //buffer to hold incoming packet
byte OutBuffer[16];	 // Array to send data back
WiFiUDP UDP;
unsigned int ReceiveFromPort = 8120;      // local port to listen on

//sending back to where and which port
IPAddress ipDestination;
unsigned int SendToPort = 2388; //port that listens

// sensors
 //ESP-01
//OneWire OWbus[2] = { OneWire(0),OneWire(2) };
//int BusCount = 2;

//// Wemos D1
OneWire OWbus[3] = { OneWire(12),OneWire(13),OneWire(14) };
int BusCount = 3;

//OneWire OWbus[1]={OneWire(5)};
//int BusCount=1;

byte SensorAddress[8];

// commands:
// 1 all sensor report
// 2 specific sensor report
// 3 set sensor userdata
// 4 read sensors
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

String apSSID = "TempMonitor " + String(ControlBoxID);
String NetworkSSID;
String NetworkPassword;

bool SendEnabled = false; // if last PGN received was for this node send is enabled

bool SensorsCounted = false;

struct SensorData
{
	byte ID[8];
	byte BusID;
	byte Temperature[2];
	byte UserData[2];
};

SensorData Sensors[255];

WiFiManager wm;

unsigned long FlashTime;
bool FlashState;

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.print(APP_Name);
	Serial.print("  :  ");
	Serial.println(App_Version);
	Serial.println("ControlBox: " + String(ControlBoxID));
	Serial.println();

	UDP.begin(ReceiveFromPort);
	delay(1000);

	WiFi.mode(WIFI_STA);
	wm.setTimeout(180);	// returns from unsuccessful AP config after this time in seconds
//	wm.resetSettings();	// reset saved settings

	wm.setWebServerCallback(bindServerCallback);

	bool ESPconnected = wm.autoConnect(apSSID.c_str());
	if (!ESPconnected) ESP.restart();

	NetworkSSID = wm.getWiFiSSID();
	NetworkPassword = wm.getWiFiPass();

	// UDP destination
	ipDestination = WiFi.localIP();
	ipDestination[3] = 255;		// change to broadcast
	Serial.println(ipDestination.toString());

	pinMode(LED_BUILTIN, OUTPUT);
}

void loop()
{
	FlashLED();

	CheckWifi();

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
}

void AllSensorsReport()
{
	for (byte i = 0; i < SensorCount; i++)
	{
		SendData(i);
	}
}

void SingleSensorReport()
{
	for (byte i = 0; i < SensorCount; i++)
	{
		if (memcmp(Sensors[i].ID, SensorAddress, 8) == 0)
		{
			SendData(i);
			break;	// exit loop
		}
	}
}

void SetNewUserData()
{
	for (byte i = 0; i < SensorCount; i++)
	{
		if (memcmp(Sensors[i].ID, SensorAddress, 8) == 0)
		{
			SetUserData(SensorAddress, Sensors[i].BusID, UserData);
			break;	// exit loop
		}
	}
}

void bindServerCallback()
{
	wm.server->on("/info", HandleTemps);
}


void FlashLED()
{
	if (millis() - FlashTime > 1000)
	{
		FlashTime = millis();
		FlashState = !FlashState;
		if (FlashState)
		{
			digitalWrite(BUILTIN_LED, HIGH);
		}
		else
		{
			digitalWrite(BUILTIN_LED, LOW);
		}
	}
}
