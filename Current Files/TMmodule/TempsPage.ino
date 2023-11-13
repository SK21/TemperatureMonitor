
void HandleTemps()
{
	float Tmp;
	unsigned long UD;
	byte BinNum;
	byte Cable;
	byte SensorNum;

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
	st += "        font-size: 18px;";
	st += "      }";
	st += "";
	st += "      table,";
	st += "      th,";
	st += "      td {";
	st += "        border: 1px solid black;";
	st += "        border-collapse: collapse;";
	st += "      }";
	st += "";
	st += "      th,";
	st += "      td {";
	st += "        padding: 5px;";
	st += "      }";
	st += "";
	st += "      th {";
	st += "        text-align: left;";
	st += "      }";
	st += "";
	st += "";
	st += "";
	st += "      table.center {";
	st += "        margin-left: auto;";
	st += "        margin-right: auto;";
	st += "      }";
	st += "";
	st += "    </style>";
	st += "";
	st += "  <body>";
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
	st += "    <h2>Sensor Temperatures</h2>";
	st += "    <table class='center' style='width:auto'>";
	st += "      <tr>";
	st += "        <th>Bin</th>";
	st += "        <th>Cable</th>";
	st += "        <th>ID</th>";
	st += "        <th>Temperature</th>";
	st += "      </tr>";

	for (byte i = 0; i < SensorCount; i++)
	{
		UD = Sensors[i].UserData[1] << 8 | Sensors[i].UserData[0];

		BinNum = UD >> 8;			// shift right to lower 8 bits
		Cable = (UD & 240) >> 4;    // remove top 8 and lower 4 bits, 0000 0000 1111 0000
		SensorNum = (UD & 15);		// remove top 12 bits, 0000 0000 0000 1111

		// convert to base 1
		BinNum += 1;
		Cable += 1;
		SensorNum += 1;

		Tmp = (float)((int16_t)(Sensors[i].Temperature[1] << 8 | Sensors[i].Temperature[0]) / 16.0);	// twos complement conversion

		st += "      <tr>";
		st += "        <td style='text-align:center'>" + String(BinNum) + "</td>";
		st += "        <td style='text-align:center'>" + String(Cable) + "</td>";
		st += "        <td style='text-align:center'>" + String(SensorNum) + "</td>";
		st += "        <td style='text-align:center'>" + String(Tmp) + "</td>";
		st += "      </tr>";
	}

	st += "    </table>";
	st += "    <br>";
	st += "    <p><a href='/'>Back</a></p>";
	st += "  </body>";
	st += "";
	st += "</HTML>";

	server.send(200, "text/html", st);
}
