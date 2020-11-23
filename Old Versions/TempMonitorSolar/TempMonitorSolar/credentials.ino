// Load WLAN credentials from EEPROM starting at byte 0
void loadCredentials()
{
  EEPROM.begin(512);
  EEPROM.get(0, clientSSID);
  EEPROM.get(0+sizeof(clientSSID), clientPassword);
  EEPROM.get(0+sizeof(clientSSID)+sizeof(clientPassword),ServerIP);
  char ok[2+1];
  EEPROM.get(0+sizeof(clientSSID)+sizeof(clientPassword)+sizeof(ServerIP), ok);
  EEPROM.end();
  if (String(ok) != String("OK"))
  {
    clientSSID[0] = 0;
    clientPassword[0] = 0;
    strcpy(DefaultServerIP,ServerIP);
  }
  Serial.println("Recovered credentials:");
  Serial.println(clientSSID);
  Serial.println(strlen(clientPassword)>0?"********":"<no password>");
  Serial.println(ServerIP);
}

/** Store WLAN credentials to EEPROM */
void saveCredentials()
{
  EEPROM.begin(512);
  EEPROM.put(0, clientSSID);
  EEPROM.put(0+sizeof(clientSSID), clientPassword);
  EEPROM.put(0+sizeof(clientSSID)+sizeof(clientPassword),ServerIP);
  char ok[2+1] = "OK";
  EEPROM.put(0+sizeof(clientSSID)+sizeof(clientPassword)+sizeof(ServerIP), ok);
  EEPROM.commit();
  EEPROM.end();
}
