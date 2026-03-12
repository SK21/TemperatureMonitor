# BinWatch User Manual

## Overview

BinWatch monitors grain bin temperatures using ESP8266-based sensor modules (BinTempsModule). Each module reads DS18B20 temperature sensors on 1-Wire buses and reports readings over WiFi/UDP to the BinWatch desktop app. Temperatures are stored in a local database and displayed in real time.

---

## BinWatch Application

### Starting the App

Run **BinWatch.exe**. On first run it creates **BinWatch.db** alongside the exe. The status bar shows the UDP port it is listening on.

The app has three tabs:

| Tab | Purpose |
|---|---|
| Dashboard | Lists all known modules and their status |
| Temperatures | Shows the latest reading per sensor |
| History | Chart of historical readings for a selected sensor |

### Dashboard

Modules appear automatically when they announce themselves over the network. Each row shows the module name, ID, IP address, status, last-seen time, and firmware version.

- **Update Temperatures** — requests a fresh sensor read from the selected module immediately
- **Identify Module** — requests the module to re-announce its name and ID
- **Settings** — configure the database path and passive mode (see below)
- **Convert Sensors** — remap sensor user-data bytes to bin/cable/sensor numbers

Double-click a module row to edit its name and ID.

### Temperatures Tab

Shows the most recent temperature for each known sensor. Columns include bin, cable, sensor number, label, temperature, and timestamp. Double-click a row to edit sensor settings (label, max temp, offset, enable/disable).

### History Tab

Select a sensor from the dropdown, set the date range, and click **Load**. The chart is also loaded automatically each time you switch to this tab.

### Settings

| Setting | Description |
|---|---|
| Database path | Where BinWatch.db is stored. Leave blank for the default (same folder as the exe). Restart the app to apply a path change. |
| Passive mode | Tick to open an existing database read-only without starting the UDP server. Use this on a secondary machine sharing a database over a network drive. |
| Debug logging | Tick to write verbose debug output to BinWatch.log. Takes effect immediately without a restart. |

---

## Sensor Modules

### First-time Setup

Each module broadcasts a WiFi access point named **ModuleName (MAC)**. Connect to it with any device and open a browser.

- Default gateway IP: **192.168.4.1**

The module's home page shows the module name and sensor count, with links to:

| Link | Purpose |
|---|---|
| Temperatures | View current readings on the module |
| Settings | Configure WiFi, module name, and hardware options |
| Refresh | Trigger an immediate temperature read |

### Settings Page

| Field | Description |
|---|---|
| Network / Password | WiFi network the module connects to for UDP reporting |
| Module Name | Shown in BinWatch (max 10 characters) |
| Use Wifi Server | Tick to enable WiFi connection; untick to run in access-point mode only |
| Use DS2482 | Tick if the module uses a DS2482 I²C 1-Wire master instead of GPIO |
| Strong pullup | Tick if a strong pullup circuit is fitted |

Click **Save/Restart** — the module restarts automatically with the new settings.

The bottom of the page shows the current WiFi connection status, signal strength, and firmware version. An **Update Firmware** link opens the OTA update page directly.

> The module ID and name are normally set from BinWatch using **Identify Module** then double-clicking the module row.

### Temperature Update Schedule

Each module reads and reports temperatures once per hour. To reduce network congestion, modules stagger their reports based on their ID:

- Module 1 reports at the top of the hour
- Module 2 reports 1 minute later
- Module 3 reports 2 minutes later, and so on

BinWatch also polls all modules every 60 minutes as a fallback, staggering commands 5 seconds apart.

---

## Firmware Update

The firmware file **BinTempsModule.ino.bin** is located in this folder alongside BinWatch.exe.

### Initial Upload (new or blank ESP8266)

A blank ESP8266 has no firmware and cannot use OTA. The first upload must be done using the **Arduino IDE** with a USB cable connected to the module:

1. Install the Arduino IDE and add ESP8266 board support via **File > Preferences > Additional Board URLs**: `http://arduino.esp8266.com/stable/package_esp8266com_index.json`
2. Open the **BinTempsModule** sketch (the `.ino` source files — not the `.bin`).
3. Select the correct board (**LOLIN(Wemos) D1 R2 & mini** or **NodeMCU 1.0**) and the COM port.
4. Click **Upload**. Once complete the module restarts and its WiFi access point becomes visible.

Subsequent updates can be done wirelessly using OTA (see below).

### OTA Update (existing module)

### Steps

1. Connect your computer to the module's WiFi access point (**ModuleName (MAC)**), or ensure you are on the same network if the module is connected via WiFi.

2. Open a browser and navigate to the module's IP address followed by **/update**:
   - Via access point: `http://192.168.4.1/update`
   - Via WiFi network: `http://<module-ip>/update`

3. The OTA update page will appear. Click **Choose File**, select **BinTempsModule.ino.bin**, then click **Update**.

4. Wait for the progress bar to complete. The module restarts automatically with the new firmware.

5. Verify the update by checking the firmware version shown in BinWatch's Dashboard tab. The version number is in **DDMMY** format, where DD = day, MM = month, and Y = last digit of the year (e.g. 10036 = 10th March 2026).

> **Note:** After a firmware update that changes the module's data structure (InoID change), the module reloads its default hardware settings. WiFi credentials are stored separately and are not affected. Re-enter the module name and hardware options (DS2482, strong pullup) via the Settings page if needed.

---

## Errors and Logging

BinWatch logs errors and key events to **BinWatch.log** in the same folder as the exe. A backup copy is kept as **BinWatch.log.bak** (kept when the log exceeds 1 MB). Check this file if the app behaves unexpectedly.

---

## File Summary

| File | Description |
|---|---|
| BinWatch.exe | Main application |
| BinWatch.db | SQLite database (modules, sensors, temperature history) |
| BinWatch.ini | App settings (database path, passive mode, window position) |
| BinWatch.log | Error and event log |
| BinTempsModule.ino.bin | Sensor module firmware binary for OTA updates |
