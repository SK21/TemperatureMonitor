void StartAP()
{
  WiFi.disconnect();
  Serial.println();
  Serial.print("Configuring access point...");
  /* You can remove the password parameter if you want the AP to be open. */
  WiFi.softAPConfig(apIP, apIP, netMsk);

  byte mac[6];
  WiFi.softAPmacAddress(mac);
  String apSSID = "TempMon " + macTostring(mac);  
  WiFi.softAP(String(apSSID).c_str(), apPassword);  // .c_str() converts the string to a constant string
  
  delay(500); // Without delay I've seen the IP address blank
  Serial.print("AP IP address: ");
  Serial.println(WiFi.softAPIP());

  /* Setup the DNS server redirecting all the domains to the apIP */  
  dnsServer.setErrorReplyCode(DNSReplyCode::NoError);
  dnsServer.start(DNS_PORT, "*", apIP);

  /* Setup web pages: root, wifi config pages, SO captive portal detectors and not found. */
  server.on("/", handleRoot);
  server.on("/doPrevious",handlePrevious);
  server.on("/doTemps",handleTemps);
  server.on("/doSave",handleSaveNetwork);
  server.on("/doHome",handleRoot);
  server.on("/doNewNetwork",handleChooseNetwork);
  server.on("/generate_204", handleRoot);  //Android captive portal. Maybe not needed. Might be handled by notFound handler.
  server.on("/fwlink", handleRoot);  //Microsoft captive portal. Maybe not needed. Might be handled by notFound handler.
  server.onNotFound ( handleNotFound );
  server.begin(); // Web server start
  Serial.println("HTTP server started");
  // Setup MDNS responder
  if (!MDNS.begin(myHostname))
  {
    Serial.println("Error setting up MDNS responder!");
  } 
  else
  {
    Serial.println("mDNS responder started");
    // Add service to MDNS-SD
    MDNS.addService("http", "tcp", 80);
  }
  apStarted=true;
}




