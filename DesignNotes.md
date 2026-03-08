# BinTemps C# App - Design Notes

## Overview

A new C# desktop application to replace the VB6 server. Communicates over WiFi (UDP) with ESP8266-based bin temperature modules (existing TM13 hardware). The existing firmware will need updates to support the new protocol.

---

## Technology Stack

| Layer | Choice |
|---|---|
| UI | WPF (.NET 8) |
| Database | SQLite via EF Core |
| Charts | LiveChartsCore or OxyPlot |
| Network | UDP (System.Net.Sockets.UdpClient) |
| Async | async/await + CancellationToken |
| Config | System.Text.Json |

---

## Project Structure

```
BinTemps/
├── BinTemps.sln
├── BinTemps.Core/              # Business logic class library
│   ├── Models/
│   │   ├── Sensor.cs
│   │   ├── TemperatureRecord.cs
│   │   ├── BinStorage.cs
│   │   ├── Module.cs
│   │   └── Packet.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   ├── Network/
│   │   ├── UdpServer.cs
│   │   └── PacketParser.cs
│   └── Services/
│       ├── TemperatureService.cs
│       ├── SensorService.cs
│       └── ReportService.cs
├── BinTemps.UI/                # WPF application
│   ├── ViewModels/
│   │   ├── MainViewModel.cs
│   │   ├── DashboardViewModel.cs
│   │   ├── SensorsViewModel.cs
│   │   ├── TemperatureViewModel.cs
│   │   └── SettingsViewModel.cs
│   └── Views/
│       ├── MainWindow.xaml
│       ├── DashboardView.xaml
│       ├── SensorsView.xaml
│       ├── TemperatureView.xaml
│       ├── BinMapView.xaml
│       └── SettingsView.xaml
└── BinTemps.Tests/
    ├── PacketParserTests.cs
    └── TemperatureServiceTests.cs
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

Sent by app on a regular interval (e.g. every 30 seconds). Modules watch for this. If 3 consecutive heartbeats are missed, module attempts WiFi reconnect and re-announces itself with PGN 30831 (ID=0).

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
- bit 2 = send sensor serials and user data (30832)

Module ID 0x00 = global broadcast - all modules respond with a random delay based on MAC address to stagger responses:
`delay_ms = (MAC[4] XOR MAC[5]) * 50`  (0 to ~12.7 seconds spread)

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
| 13 | Status (bit 0 = finished sending all temps) |
| 14 | CRC |

One packet per sensor. Final packet in sequence has status bit 0 set.

#### 30831 - Module Description
| Byte | Field |
|---|---|
| 0 | 111 |
| 1 | 120 |
| 2 | Module ID (0 = unregistered) |
| 3-8 | Module MAC (6 bytes) |
| 9-18 | Module description (10 bytes, UTF-8) |
| 19 | CRC |

Sent unsolicited once on startup/reconnect. If module ID = 0, server knows it is unregistered and prompts user to assign an ID via 30822. Also sent in response to 30821 command bit 1.

#### 30832 - Sensor Description
| Byte | Field |
|---|---|
| 0 | 112 |
| 1 | 120 |
| 2 | Module ID |
| 3-10 | Sensor serial / ROM code (8 bytes) |
| 11 | Sensor user data byte 0 |
| 12 | Sensor user data byte 1 |
| 13 | Status (bit 0 = finished sending all sensor data) |
| 14 | CRC |

One packet per sensor. Final packet has status bit 0 set.

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
1. Module misses 3 consecutive heartbeats
2. Module attempts WiFi reconnect
3. On reconnect, module sends unsolicited 30831 again (with its assigned ID)
4. Server knows the module is back online

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

## Implementation Phases

1. **Phase 1** - PacketParser + unit tests, UdpServer, console test harness against real hardware
2. **Phase 2** - EF Core models, TemperatureService, SensorService
3. **Phase 3** - Main WPF window, Dashboard (live connections), Temperature sheet
4. **Phase 4** - Chart view, Bin map, CSV export
5. **Phase 5** - Settings UI, Sensor management, Installer

---

## Open Questions / Future Considerations

- Add full timestamp (hour/min/date) to heartbeat 30820 if modules ever need to self-timestamp queued data while server is offline
- ACK/NAK response from module for 30822/30823 write commands (not yet defined)
- Sensor count in 30831 so server knows how many 30830/30832 packets to expect
- Error/diagnostic reporting PGN (1-Wire bus failures, CRC error counts, WiFi RSSI)
- OTA firmware update PGN
