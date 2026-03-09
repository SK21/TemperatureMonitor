using System;
using System.Net;
using System.Text;

namespace BinTempsApp.Models
{
    public class TemperaturePacket
    {
        public byte ModuleId { get; }
        public byte[] RomCode { get; }          // 8 bytes, DS18B20 serial
        public short RawTemperature { get; }    // signed 16-bit DS18B20 value
        public float Temperature { get; }       // RawTemperature / 16.0
        public byte UserData0 { get; }
        public byte UserData1 { get; }
        public byte SensorsRemaining { get; }
        public IPEndPoint Source { get; }

        // Decoded user data: [15:8] Bin | [7:4] Cable | [3:0] Sensor
        public byte BinId => UserData1;
        public byte CableId => (byte)((UserData0 >> 4) & 0xF);
        public byte SensorNum => (byte)(UserData0 & 0xF);

        public string RomCodeHex => BitConverter.ToString(RomCode).Replace("-", "");

        public TemperaturePacket(byte moduleId, byte[] romCode, short rawTemperature,
            byte userData0, byte userData1, byte sensorsRemaining, IPEndPoint source)
        {
            ModuleId = moduleId;
            RomCode = romCode;
            RawTemperature = rawTemperature;
            Temperature = rawTemperature / 16.0f;
            UserData0 = userData0;
            UserData1 = userData1;
            SensorsRemaining = sensorsRemaining;
            Source = source;
        }
    }

    public class ModuleDescriptionPacket
    {
        public byte ModuleId { get; }           // 0 = unregistered
        public byte[] Mac { get; }              // 6 bytes
        public string Name { get; }             // up to 10 chars UTF-8
        public ushort FirmwareVersion { get; }  // InoID (DDMMY format)
        public IPEndPoint Source { get; }

        public string MacString => BitConverter.ToString(Mac).Replace("-", ":");
        public bool IsUnregistered => ModuleId == 0;

        public ModuleDescriptionPacket(byte moduleId, byte[] mac, string name,
            ushort firmwareVersion, IPEndPoint source)
        {
            ModuleId = moduleId;
            Mac = mac;
            Name = name;
            FirmwareVersion = firmwareVersion;
            Source = source;
        }
    }
}
