
String GetPage0()
{
	String st = "<HTML>";
	st += "";
	st += "  <head>";
	st += "    <META content='text/html; charset=utf-8' http-equiv=Content-Type>";
	st += "    <meta name=vs_targetSchema content='HTML 4.0'>";
	st += "    <meta name='viewport' content='width=device-width, initial-scale=1.0'>";
	st += "    <title>Temp Monitor</title>";
	st += "    <style>";
	st += "      html {";
	st += "        font-family: Helvetica;";
	st += "        display: inline-block;";
	st += "        margin: 0px auto;";
	st += "        text-align: center;";
	st += "";
	st += "      }";
	st += "";
	st += "      h1 {";
	st += "        color: #444444;";
	st += "        margin: 50px auto 30px;";
	st += "      }";
	st += "";
	st += "      table.center {";
	st += "        margin-left: auto;";
	st += "        margin-right: auto;";
	st += "      }";
	st += "";
	st += "      .buttonOn {";
	st += "        background-color: #00ff00;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 30%;";
	st += "      }";
	st += "";
	st += "      .buttonOff {";
	st += "        background-color: #ff0000;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 30%;";
	st += "      }";
	st += "";
	st += "      .button-72 {";
	st += "        align-items: center;";
	st += "        background-color: initial;";
	st += "        background-image: linear-gradient(rgba(179, 132, 201, .84), rgba(57, 31, 91, .84) 50%);";
	st += "        border-radius: 42px;";
	st += "        border-width: 0;";
	st += "        box-shadow: rgba(57, 31, 91, 0.24) 0 2px 2px, rgba(179, 132, 201, 0.4) 0 8px 12px;";
	st += "        color: #FFFFFF;";
	st += "        cursor: pointer;";
	st += "        display: flex;";
	st += "        font-family: Quicksand, sans-serif;";
	st += "        font-size: 18px;";
	st += "        font-weight: 700;";
	st += "        justify-content: center;";
	st += "        letter-spacing: .04em;";
	st += "        line-height: 16px;";
	st += "        margin: auto;";
	st += "        padding: 18px 18px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        text-shadow: rgba(255, 255, 255, 0.4) 0 0 4px, rgba(255, 255, 255, 0.2) 0 0 12px, rgba(57, 31, 91, 0.6) 1px 1px 4px, rgba(57, 31, 91, 0.32) 4px 4px 16px;";
	st += "        user-select: none;";
	st += "        -webkit-user-select: none;";
	st += "        touch-action: manipulation;";
	st += "        vertical-align: baseline;";
	st += "        width:30%";
	st += "      }";
	st += "";
	st += "      .InputCell {";
	st += "        text-align: center;";
	st += "      }";
	st += "";
	st += "    </style>";
	st += "  </head>";
	st += "";
	st += "  <BODY>";
	st += "    <style>";
	st += "      body {";
	st += "        margin-top: 50px;";
	st += "        background-color: wheat";
	st += "      }";
	st += "";
	st += "      font-family: Arial,";
	st += "      Helvetica,";
	st += "      Sans-Serif;";
	st += "";
	st += "    </style>";
	st += "";
	st += "    <h1 align=center>Temp Monitor";
	st += "    </h1>";
	st += "";
	st += "    <h1 align=center>" + String(MDL.Name);
	st += "    </h1>";
	st += "    <h1 align=center>Sensor Count: " + String(SensorCount);
	st += "    </h1>";
	st += "    <h1 align=center>Server RSSI: " + String(SignalStrength());
	st += "    </h1>";
	st += "    <form id=FORM1 method=post action='/'>&nbsp;";
	st += "";
	st += "";
	st += "      <p> <a class='button-72' href='/page1' >Temperatures</a> </p>";
	st += "      <p> <a class='button-72' href='/page2' >Settings</a> </p>";
	st += "      <p> <a class='button-72' href='/update' >Refresh</a> </p>";
	st += "";
	st += "    </form>";
	st += "";
	st += "</HTML>";

	return st;
}

int8_t SignalStrength()
{
	int8_t Result = 0;
	if (WiFi.RSSI() < 0) Result = WiFi.RSSI();
	return Result;
}
