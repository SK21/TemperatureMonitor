unsigned long UD;
float Tmp;
byte BinNum;
byte Cable;
byte SensorNum;

void HandleTemps()
{
    wm.server->sendHeader("Cache-Control", "no-cache, no-store, must-revalidate");
    wm.server->sendHeader("Pragma", "no-cache");
    wm.server->sendHeader("Expires", "-1");
    wm.server->setContentLength(CONTENT_LENGTH_UNKNOWN);
    wm.server->send(200, "text/html", ""); // Empty content inhibits Content-length header so we have to close the socket ourselves.
    wm.server->sendContent(
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

    UpdateSensors();

    for (byte i = 0; i < SensorCount; i++)
    {
        UD = Sensors[i].UserData[0] << 8 | Sensors[i].UserData[1];
        BinNum = UD >> 8;   // shift right to lower 8 bits
        Cable = (UD & 240) >> 4;    // remove top 8 and lower 4 bits, 0000 0000 1111 0000
        SensorNum = (UD & 15);  // remove top 12 bits, 0000 0000 0000 1111

        Tmp = (float)((Sensors[i].Temperature[0] << 8 | Sensors[i].Temperature[1]) / 10.0);

        wm.server->sendContent(
            //String() + "<tr><td>Sensor ID  [" + Rom + "]"
            String() + "<tr><td>"
            "  Bin " + BinNum + ","
            "  Cable " + Cable + ","
            "  Sensor " + SensorNum + ","
            "  Temp " + Tmp + ""
            "</td></tr><br><br>"
        );
    }

    wm.server->sendContent(
        "</table>"
        "<p>Finished.</p>"
        "</body></html>"
    );
    wm.server->client().stop(); // Stop is needed because we sent no content length
}

