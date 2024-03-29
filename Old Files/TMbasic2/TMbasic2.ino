# define InoDescription "TMbasic2   7-Dec-2022"
// Wemos D1 mini Pro,  board: LOLIN(Wemos) D1 R2 & mini

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

#include <OneWire.h>            // https://github.com/PaulStoffregen/OneWire
#include <WiFiClient.h> 
#include <ESP8266WiFi.h>        // Arduino IDE 'Additional Board' http://arduino.esp8266.com/stable/package_esp8266com_index.json
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <ArduinoOTA.h>
#include <EEPROM.h>
#include <Wire.h>
#include "DS2482.h" 		    // https://github.com/paeaetech/paeae

#define SDApin  4
#define SCLpin  5

#define MaxSensors 200

struct ModuleData
{
    char Name[20] = "Module0";
    char SSID[32] = "ssid";
    char Password[32] = "password";
    IPAddress ServerIP = IPAddress(192,168,1,1);
    uint16_t Port = 1600;
    byte Pin1 = 4;
    byte Pin2 = 5;
    bool UseDS2482 = false;
};

ModuleData MDL;

OneWire OWbus[] = { OneWire(MDL.Pin1),OneWire(MDL.Pin2)};
byte BusCount = 2;

DS2482 OneWireMaster(0);

struct SensorData
{
    byte ID[8];
    byte BusID;
    byte Temperature[2];	// msb [1], lsb [0]
    byte UserData[2];
};

SensorData Sensors[MaxSensors];

#define CheckValue 4500
int16_t DataCheck;
unsigned long LoopTime;

const long pulsewait = 60000;                       // time to wait for a pulse before attempting to reconnect (milliseconds)
unsigned long pulsetime = millis() - pulsewait;     // set to connect to server on startup

WiFiClient client;

int maxread = 50;   // maximum # of characters to read from server
String packetin = "";
String packetout = "";
int P1 = 0;       // packet type
String P2 = "";   // packet data
String P3 = "";   // packet sensor address

byte RomCode[8];    // for sensor address
byte SensorCount = 0;

int UserData;
uint8_t ErrorCount;
bool DS2842Connected = false;
ESP8266WebServer server(80);

void setup()
{
    Serial.begin(115200);
    delay(5000);
    Serial.println();
    Serial.println(InoDescription);
    Serial.println();

    EEPROM.begin(512);
    EEPROM.get(0, DataCheck);
    if (DataCheck == CheckValue)
    {
        EEPROM.get(10, MDL);
    }
    else
    {
        EEPROM.put(0, CheckValue);
        EEPROM.put(10, MDL);
        EEPROM.commit();
    }

    StartOTA();

    if (MDL.UseWifi)
    {
        CheckWifi();
    }
    else
    {
        Serial.println("Not using Wifi");
    }

    String AP = "TempMon   ";
    if (String(MDL.Name) == "Module0")
    {
        AP += WiFi.macAddress();
    }
    else
    {
        AP += String(MDL.Name);
    }

    WiFi.softAP(AP);

    Serial.print("Access Point name: ");
    Serial.println(AP);

    // web server
    Serial.println();
    Serial.println("Starting Web Server");
    server.on("/", HandleRoot);
    server.on("/page1", HandleTemps);
    server.on("/page2", HandlePage2);
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

    if (DS2842Connected)
    {
        UpdateSensorsMaster();
    }
    else
    {
        UpdateSensors();
    }

    Serial.println("");

    checkserver();

    pinMode(LED_BUILTIN, OUTPUT);

    Serial.println("");
    Serial.println("Finished Setup");
    Serial.println("");
}

void loop()
{
    ArduinoOTA.handle();
    server.handleClient();

    if (MDL.UseWifi)
    {
        if (millis() - LoopTime > 10000)
        {
            LoopTime = millis();
            checkserver();    //check server connected
            if (WiFi.status() != WL_CONNECTED) CheckWifi();
        }

        readserver();     //check for message from server
        checkchunks();    //break up and process message
    }

    Blink();
}

bool State = 0;
uint32_t BlinkTime;
uint32_t LastBlink;

void Blink()
{
    if (millis() - BlinkTime > 1000)
    {
        State = !State;
        digitalWrite(LED_BUILTIN, State);
        BlinkTime = millis();
        //Serial.print(" Loop interval (ms): ");
        //Serial.println((float)(micros() - LastBlink) / 1000.0, 3);
    }
    LastBlink = micros();
}

String IPtoString(IPAddress addr)
{
    String ans = String(addr[0]) + "." + String(addr[1]) + "." + String(addr[2]) + "." + String(addr[3]);
    return ans;
}

String addressTostring(byte addr[8])    //converts 8 byte address to hex and places a space between bytes
{
    String ans = "";
    for (byte i = 0; i < 8; i++)
    {
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
