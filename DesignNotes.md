# BinTemps C# App - Design Notes

## Overview

A new C# desktop application to replace the VB6 server. Communicates over WiFi (UDP) with ESP8266-based bin temperature modules (existing TM13 hardware). Firmware is complete.

---

## Technology Stack

| Layer | Choice |
|---|---|
| UI | WinForms (.NET Framework 4.8) |
| Database | SQLite via EF6 (`EntityFramework` + `System.Data.SQLite`) |
| Charts | OxyPlot (`OxyPlot.WindowsForms`) |
| Network | UDP (`System.Net.Sockets.UdpClient`) |
| Async | async/await + CancellationToken |
| Config | Newtonsoft.Json |

---

## Project Structure

```
BinTempsApp/
├── BinTempsApp.slnx
└── BinTempsApp/
    ├── BinTempsApp.csproj        # .NET Framework 4.8 WinForms
    ├── Program.cs
    ├── MainForm.cs               # Main MDI or tabbed shell
    ├── Network/
    │   ├── UdpServer.cs          # Bind, heartbeat broadcast, receive loop
    │   └── PacketParser.cs       # Parse raw bytes into typed packets
    ├── Models/
    │   ├── Module.cs
    │   ├── Sensor.cs
    │   ├── TemperatureRecord.cs
    │   └── Packet.cs
    ├── Data/
    │   ├── AppDbContext.cs       # EF6 DbContext
    │   └── Migrations/
    ├── Services/
    │   ├── TemperatureService.cs
    │   └── SensorService.cs
    └── Forms/
        ├── DashboardForm.cs      # Live module/connection status
        ├── TemperatureForm.cs    # Temperature data sheet
        ├── ChartForm.cs          # OxyPlot trend charts
        ├── BinMapForm.cs         # Visual bin layout
        └── SettingsForm.cs       # App settings, sensor management
```

---

## UDP Protocol (PGNs)

All packets begin with two fixed bytes: type identifier and protocol version (120).
Temperature lo/hi = raw DS18B20 16-bit value (signed, divide by 16.0 for Celsius).

### App to Module

#### 30820 - Heartbeat (broadcast, 3 bytes)
| Byte | Value |
|---|---|
| 0 | 100 |
| 1 | 120 |
| 2 | CRC |

Sent by app on a regular interval (e.g. every 30 seconds) to the subnet broadcast address. Modules track the last heartbeat time. If no heartbeat is received for 90 seconds (~3 missed), the module attempts WiFi reconnect and re-announces itself with PGN 30831.

#### 30821 - Command (app to module)
| Byte | Field |
|---|---|
| 0 | 101 |
| 1 | 120 |
| 2 | Module ID (0x00 = global broadcast) |
| 3 | Command byte |
| 4 | CRC |

Command byte bits:
- bit 0 = send sensor temps (30830)
- bit 1 = send module description (30831)

Module ID 0x00 = global broadcast — all modules respond.
Module ID 0x01-0xFE = targeted, module responds immediately.
Module ID 0xFF = reserved.

#### 30822 - Set Module Description (app to module)
| Byte | Field |
|---|---|
| 0 | 102 |
| 1 | 120 |
| 2-7 | Module MAC (6 bytes, used to identify target) |
| 8 | New Module ID |
| 9-18 | New Description (10 bytes, UTF-8) |
| 19 | CRC |

#### 30823 - Set Sensor User Data (app to module)
| Byte | Field |
|---|---|
| 0 | 103 |
| 1 | 120 |
| 2 | Module ID |
| 3-10 | Sensor serial / ROM code (8 bytes) |
| 11 | New user data byte 0 |
| 12 | New user data byte 1 |
| 13 | CRC |

---

### Module to App

#### 30830 - Temperature Data
| Byte | Field |
|---|---|
| 0 | 110 |
| 1 | 120 |
| 2 | Module ID |
| 3-10 | Sensor serial / ROM code (8 bytes) |
| 11 | Temp lo (raw DS18B20 low byte) |
| 12 | Temp hi (raw DS18B20 high byte) |
| 13 | Sensor user data byte 0 |
| 14 | Sensor user data byte 1 |
| 15 | Count of sensors remaining |
| 16 | CRC |

One packet per sensor. Sensor user data (bin/cable/sensor encoding) is included inline with each reading. Count of sensors remaining decrements to 0 on the final packet.

#### 30831 - Module Description
| Byte | Field |
|---|---|
| 0 | 111 |
| 1 | 120 |
| 2 | Module ID (0 = unregistered) |
| 3-8 | Module MAC (6 bytes) |
| 9-18 | Module description (10 bytes, UTF-8) |
| 19-20 | InoID / firmware version (uint16, lo byte first) |
| 21 | CRC |

Sent unsolicited once on startup/reconnect. If module ID = 0, server knows it is unregistered and prompts user to assign an ID via 30822. Also sent in response to 30821 command bit 1. The InoID field allows the app to detect outdated firmware.

---

## Sensor User Data Encoding

16-bit value stored in DS18B20 EEPROM bytes 2-3 (unchanged from original firmware):
```
[15:8] = Bin ID    [7:4] = Cable ID    [3:0] = Sensor number
```

```csharp
public static (byte Bin, byte Cable, byte Sensor) DecodeUserData(ushort raw)
    => ((byte)(raw >> 8), (byte)((raw >> 4) & 0xF), (byte)(raw & 0xF));
```

---

## WiFi Provisioning

Modules are physically installed in the field (grain bins), often too far from the server PC to reliably connect to the module's soft-AP hotspot. Provisioning is therefore done via a smartphone brought to the bin.

### Provisioning Flow
1. Module boots with no stored credentials (or fails to connect after N attempts)
2. Module starts a soft-AP named `BinTemps-<MAC>` and serves a simple config web page
3. Technician walks to the bin with a smartphone, connects to the module's AP
4. Browser opens the config page, technician enters SSID + password
5. Module saves credentials to EEPROM and reboots into station mode
6. On reconnect, module sends unsolicited 30831 — the C# app handles registration from there

The C# app has no role in provisioning. It only becomes involved once the module is on the network.

---

## Module Identity & Discovery

- **MAC address** is the stable unique key for each module (factory-burned, never changes).
- **Module ID** (1 byte, 1-254) is assigned by the server via 30822 and stored in module EEPROM.
- **ID = 0** means unregistered.

### New Module Flow
1. Module powers up with ID = 0
2. Module sends one unsolicited 30831 (MAC + ID=0)
3. Server detects ID=0, prompts user to assign name/ID
4. Server sends 30822 using MAC as key, assigns new ID + description
5. Module stores ID in EEPROM, uses it from then on
6. If 30822 is lost, the next global 30821 (broadcast) will re-trigger the 30831 from the module

### Reconnect Flow
1. Module receives no heartbeat for 90 seconds
2. If WiFi is disconnected, module attempts reconnect
3. On reconnect, `udp.begin(port)` is called and module sends unsolicited 30831 (with its assigned ID)
4. If WiFi is still connected but server went quiet, module re-sends 30831 to re-announce itself
5. Server knows the module is back online

---

## Database Schema (SQLite)

```
Modules      - MacAddress (PK), ModuleId, Name, LastKnownIp, LastSeen, Status
Sensors      - RomCode (PK), ModuleMac (FK), BinId, CableId, SensorNum, Enabled, Offset, MaxTemp, Label
Records      - Id, RomCode (FK), Temperature, Timestamp
Bins         - Id, Name, Description, Rows, Cols
BinSensorMap - BinId, SensorRomCode, Row, Col
Settings     - Key, Value
```

---

## C# App — UDP Notes

- App binds to `0.0.0.0` on port 1600 to receive UDP from any module on the network
- Heartbeat (30820) and commands (30821) are sent to the subnet broadcast address
- Module responses (30830, 30831) arrive from the module's IP — use `UdpReceiveResult.RemoteEndPoint` to track module IPs
- `UdpClient` must have `EnableBroadcast = true` and `Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true)`
- Since modules broadcast their responses, the app may receive its own sent packets — filter by checking the source IP is not the local machine

## Implementation Phases

1. **Phase 1** *(start here)* — `UdpServer` (bind, broadcast heartbeat, receive loop), `PacketParser` with unit tests, console test harness against real hardware
2. **Phase 2** — EF Core models (`AppDbContext`, migrations), `TemperatureService`, `SensorService`
3. **Phase 3** — Main WPF window, Dashboard (live module list, online/offline status), Temperature sheet
4. **Phase 4** — Chart view, Bin map, CSV export
5. **Phase 5** — Settings UI, Sensor management (assign bin/cable/sensor), Installer

---

## Open Questions / Future Considerations

- Add full timestamp (hour/min/date) to heartbeat 30820 if modules ever need to self-timestamp queued data while server is offline
- ACK/NAK response from module for 30822/30823 write commands (not yet defined)
- Error/diagnostic reporting PGN (1-Wire bus failures, CRC error counts, WiFi RSSI)
- OTA firmware update PGN
