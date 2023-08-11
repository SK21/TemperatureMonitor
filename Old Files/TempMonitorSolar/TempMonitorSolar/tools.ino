/** Is this an IP? */
boolean isIp(String str) {
  for (int i = 0; i < str.length(); i++) {
    int c = str.charAt(i);
    if (c != '.' && (c < '0' || c > '9')) {
      return false;
    }
  }
  return true;
}

/** IP to String? */
String toStringIp(IPAddress ip) {
  String res = "";
  for (int i = 0; i < 3; i++) {
    res += String((ip >> (8 * i)) & 0xFF) + ".";
  }
  res += String(((ip >> 8 * 3)) & 0xFF);
  return res;
}

String macTostring(byte addr[6])  // convert 6 byte address
{
  String ans="";
  for( byte i=0; i< 6; i++)
  {
    delay(100);
    ans=ans + DecToHex(addr[i]);
    ans=ans + " ";
  }
  return ans;
}

void CheckMode()
{
  // check which connect mode TM is in
  //connect mode, 0 = access point, 1 = client, 2 = push
//  Serial.println("CheckMode");
  NewConMode = 0;
  // check for saved data, required for other two modes
  if (strlen(clientSSID) > 0)
  {
    if (digitalRead(ClientPin) == LOW)
    {
      // use client mode
      NewConMode = 1;
    }
    else if (digitalRead(PushPin) == LOW)
    {
      NewConMode = 2;
    }
  }
  if (ConMode != NewConMode)
  {
    // change modes
    client.stop();
    server.stop(); 
    WiFi.disconnect();
    PulseInit = true;
    PushCount = 0;
    delay(1000);
    if (NewConMode == 0)
    {
      RadioOn();
      WiFi.mode(WIFI_AP);
      ResetSleep; // resets sleep cycle flag so next time push mode is used the sleep cycle starts from the beginning
    }
    else if (NewConMode == 1)
    {
      RadioOn(); 
      WiFi.mode(WIFI_STA);
      ResetSleep;
    }
    else
    {
      WiFi.mode(WIFI_STA);
    }
    ConMode = NewConMode;
    Serial.println();
    Serial.println("Configuring ConMode = " + String(ConMode));
  }
//  Serial.println("ConMode = " + String(ConMode));
}

void FlashPin()
{
  digitalWrite(LEDpin, HIGH);
  delay(750);
  digitalWrite(LEDpin, LOW);
  delay(750);
}

boolean ServerIsConnected()
{
  if (PulseInit)
  {
    // still initializing, not connected
    return false;
  }
  else
  {
    if (pulsewait > (millis() - pulsetime))
    {
      return true;
    }
    else
    {
      return false;
    }    
  }
}

