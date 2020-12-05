#include <WiFiManager.h>		// https://github.com/tzapu/WiFiManager
#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiUdp.h>
#include <EEPROM.h>

#define APP_Name "Temperature Monitor"
#define App_Version "04-Dec-2020"

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
//// Wemos D1
OneWire OWbus[3] = { OneWire(12),OneWire(13),OneWire(14) };
int BusCount = 3;

 //ESP-01
//OneWire OWbus[2] = { OneWire(0),OneWire(2) };
//int BusCount = 2;

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

String apSSID;
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

bool SaveParameters;

byte ControlBoxID;	// unique to each control box
char CBID[5];		// controlbox ID char

void setup()
{
	Serial.begin(38400);

	LoadParameters();

	delay(5000);
	Serial.println();
	Serial.print(APP_Name);
	Serial.print("  :  ");
	Serial.println(App_Version);
	Serial.println("ControlBox: " + String(ControlBoxID));
	Serial.println();

	UDP.begin(ReceiveFromPort);
	delay(1000);

	apSSID = "TempMonitor " + String(ControlBoxID);

	WiFi.mode(WIFI_STA);
	wm.setTimeout(180);	// returns from unsuccessful AP config after this time in seconds
	//wm.resetSettings();	// reset saved settings

	wm.setWebServerCallback(bindServerCallback);
	wm.setSaveConfigCallback(SaveConfigCallback);

	String ID = String(ControlBoxID);
	ID.toCharArray(CBID, 5);

	WiFiManagerParameter BoxID("ID", "Unique Controlbox ID:", CBID, 5);
	wm.addParameter(&BoxID);

	std::vector<const char*> menu = { "wifi","info","param","exit" };
	wm.setMenu(menu);

	bool ESPconnected = wm.autoConnect(apSSID.c_str());
	if (!ESPconnected) ESP.restart();

	NetworkSSID = wm.getWiFiSSID();
	NetworkPassword = wm.getWiFiPass();

	strcpy(CBID, BoxID.getValue());
	if (SaveParameters) SaveNewParameters();

	// UDP destination
	ipDestination = WiFi.localIP();
	ipDestination[3] = 255;		// change to broadcast

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

void SaveConfigCallback()
{
	SaveParameters = true;
}

void SaveNewParameters()
{
	ControlBoxID = atoi(CBID);
	SaveParameters = false;

	EEPROM.begin(512);
	EEPROM.put(0, ControlBoxID);
	EEPROM.commit();
	EEPROM.end();
}

void LoadParameters()
{
	EEPROM.begin(512);
	EEPROM.get(0, ControlBoxID);
	EEPROM.end();
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

