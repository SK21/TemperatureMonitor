/** Handle root or redirect to captive portal */
void handleRoot()
{
  if (captivePortal())
  { // If caprive portal redirect instead of displaying the page.
    return;
  }
  server.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
  server.sendHeader("Pragma", "no-cache");
  server.sendHeader("Expires", "-1");
  server.setContentLength(CONTENT_LENGTH_UNKNOWN);
  server.send(200, "text/html", ""); // Empty content inhibits Content-length header so we have to close the socket ourselves.
  server.sendContent(
    "<html><head>"
    "<style>"
    "h1 {text-align:left;font-size:80px;}"
    "p {font-size:70px;}"
    "a {font-size:70px;}"
    "</style>"
    "</head><body>"
    "<h1>Temperature Monitor</h1>"
    "<br /><a href='doNewNetwork'>Connect to New Network</a>"
    "<br /><a href='doPrevious'>Use Previous Network</a>"
    "<br /><a href='doTemps'>Temperatures</a>"
    "</body></html>" 
    ); 
  server.client().stop(); // Stop is needed because we sent no content length
}



void handleChooseNetwork() {
  server.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
  server.sendHeader("Pragma", "no-cache");
  server.sendHeader("Expires", "-1");
  server.setContentLength(CONTENT_LENGTH_UNKNOWN);
  server.send(200, "text/html", ""); // Empty content inhibits Content-length header so we have to close the socket ourselves.
  server.sendContent(
    "<html><head>"
    "<style>"
    "h1 {text-align:left;font-size:80px;}"
    "p {font-size:70px;}"
    "table {font-size:70px;}"
    "a {font-size:70px;}"
    "form {font-size:70px;}"
    "input {font-size:70px;}"
    "</style>"
    "</head><body>"
    "<h1>Temperature Monitor</h1>"
    "</table>"
    "\r\n<br />"
    "<table><tr><th align='left'>Network list</th></tr>"
  );
  Serial.println("scan start");
  int n = WiFi.scanNetworks();
  Serial.println("scan done");
  if (n > 0)
  {
    for (int i = 0; i < n; i++)
    {
      Serial.println("Found " + WiFi.SSID(i));
      server.sendContent(String() + "<tr><td>SSID  " +WiFi.SSID(i) + "  (" + WiFi.RSSI(i) + ")</td></tr>");
    }
  }
  server.sendContent(
    "</table>"
    "<br /><form method='POST' action='doSave'><h4>Connect to new network:</h4>"
    "<input type='text' placeholder='network' name='n'/>"
    "<br /><input type='password' placeholder='password' name='p'/>"
    "<br /><input type='text' placeholder='server IP' name='s'/>"
    "<br /><input type='submit' value='Connect'/></form>"
    "<br /><a href='doHome'>Home</a>"
    "</body></html>"
  );
  server.client().stop(); // Stop is needed because we sent no content length
}



/** Redirect to captive portal if we got a request for another domain. Return true in that case so the page handler do not try to handle the request again. */
boolean captivePortal() {
  if (!isIp(server.hostHeader()) && server.hostHeader() != (String(myHostname)+".local")) {
    Serial.print("Request redirected to captive portal");
    server.sendHeader("Location", String("http://") + toStringIp(server.client().localIP()), true);
    server.send ( 302, "text/plain", ""); // Empty content inhibits Content-length header so we have to close the socket ourselves.
    server.client().stop(); // Stop is needed because we sent no content length
    return true;
  }
  return false;
}



/** Handle the WLAN save form and redirect to WLAN config page again */
void handleSaveNetwork() {
  Serial.println("wifi save");
  server.arg("n").toCharArray(clientSSID, sizeof(clientSSID) - 1);
  server.arg("p").toCharArray(clientPassword, sizeof(clientPassword) - 1);
  server.arg("s").toCharArray(ServerIP, sizeof(ServerIP)-1);
  Serial.println("clientSSID = " + String(clientSSID));
  Serial.println("clientPassword = " + String(clientPassword));
  Serial.println("ServerIP = " + String(ServerIP));
  server.sendHeader("Location", "doHome", true);
  server.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
  server.sendHeader("Pragma", "no-cache");
  server.sendHeader("Expires", "-1");
  server.send ( 302, "text/plain", "");  // Empty content inhibits Content-length header so we have to close the socket ourselves.
  server.client().stop(); // Stop is needed because we sent no content length
  saveCredentials();
}



void handleNotFound() {
  if (captivePortal()) { // If caprive portal redirect instead of displaying the error page.
    return;
  }
  String message = "File Not Found\n\n";
  message += "URI: ";
  message += server.uri();
  message += "\nMethod: ";
  message += ( server.method() == HTTP_GET ) ? "GET" : "POST";
  message += "\nArguments: ";
  message += server.args();
  message += "\n";
  for ( uint8_t i = 0; i < server.args(); i++ ) {
    message += " " + server.argName ( i ) + ": " + server.arg ( i ) + "\n";
  }
  server.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
  server.sendHeader("Pragma", "no-cache");
  server.sendHeader("Expires", "-1");
  server.send ( 404, "text/plain", message );
}



void handlePrevious()
{
  loadCredentials();
}



void handleTemps()
{
  server.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
  server.sendHeader("Pragma", "no-cache");
  server.sendHeader("Expires", "-1");
  server.setContentLength(CONTENT_LENGTH_UNKNOWN);
  server.send(200, "text/html", ""); // Empty content inhibits Content-length header so we have to close the socket ourselves.
  server.sendContent(
    "<html><head>"
    "<style>"
    "h1 {text-align:left;font-size:80px;}"
    "p {font-size:70px;}"
    "table {font-size:50px;}"
    "a {font-size:70px;}"
    "</style>"
    "</head><body>"
    "<h1>Temperature Monitor</h1>"
    "<p>Getting temperatures ...</p>"
    "</table>"
    "\r\n<br />"
    "<table><tr><th align='left'>Sensor Temperatures</th></tr>"
  );
  handleTempsBus1();
  handleTempsBus2();
  server.sendContent(
    "</table>"
    "<p>Finished.</p>"
    "<br /><a href='doHome'>Home</a>"
    "</body></html>"
  );
  server.client().stop(); // Stop is needed because we sent no content length
}

