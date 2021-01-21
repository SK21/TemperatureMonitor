String GetPage1()
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
	st += "      .InputCell {";
	st += "        text-align: center;";
	st += "      }";
	st += "";
	st += "      .button {";
	st += "        background-color: #555555;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 20%;";
	st += "      }";
	st += "";
	st += "    </style>";
	st += "  </head>";
	st += "";
	st += "  <BODY>";
	st += "    <style>";
	st += "      body {";
	st += "        margin-top: 50px;";
	st += "        background-color: DeepSkyBlue";
	st += "      }";
	st += "";
	st += "      font-family: Arial,";
	st += "      Helvetica,";
	st += "      Sans-Serif;";
	st += "";
	st += "    </style>";
	st += "";
	st += "    <h1 align=center>Temperature Controlbox</h1>";
	st += "    <form id=FORM1 method=post action='/'>&nbsp;";
	st += "";
	st += "      <table class='center'>";
	st += "        <tr>";
	st += "          <td align='left'>Network</td>";
	st += "          <td><input class='InputCell' size='25' name='prop1' value='" + String(BoxData.SSID) + "' ID=Text1></td>";
	st += "        </tr>";
	st += "        <tr>";
	st += "          <td align='left'>Password </td>";
	st += "          <td><input class='InputCell' size='25' name='prop2' value='" + String(BoxData.Password) + "' ID=Text2></td>";
	st += "        </tr>";
	st += "        <tr>";
	st += "          <td align='left'>ID </td>";
	st += "          <td><input class='InputCell' size='25' name='prop3' value='" + String(BoxData.ID) + "' ID=Text3></td>";
	st += "        </tr>";
	st += "        <tr>";
	st += "          <td align='left'>Sleep Interval (minutes) </td>";
	st += "          <td><input class='InputCell' size='25' name='prop4' value='" + String(BoxData.SleepInterval) + "' ID=Text4></td>";
	st += "        </tr>";
	st += "      </table>";
	st += "      <p> <input class='button' id=Submit1 type=submit value='Save'> </p>";
	st += "    </form>";
	st += "    <br>";
	st += "    <p><a href='/Temps'>Current Temperatures</a></p>";
	st += "  </BODY>";
	st += "";
	st += "</HTML>";
	return st;
}

