using System;
using System.Net;
using System.Text;
using BinTempsApp.Models;

namespace BinTempsApp.Network
{
    public class PacketParser
    {
        public event EventHandler<TemperaturePacket> TemperatureReceived;
        public event EventHandler<ModuleDescriptionPacket> ModuleDescriptionReceived;

        // Subscribe this to UdpServer.PacketReceived
        public void HandlePacket(object sender, PacketReceivedEventArgs e)
        {
            Parse(e.Data, e.RemoteEndPoint);
        }

        public void Parse(byte[] data, IPEndPoint source)
        {
            if (data == null || data.Length < 3) return;

            int pgn = (data[1] << 8) | data[0];

            switch (pgn)
            {
                case 30830:
                    ParseTemperature(data, source);
                    break;

                case 30831:
                    ParseModuleDescription(data, source);
                    break;
            }
        }

        private void ParseTemperature(byte[] data, IPEndPoint source)
        {
            // PGN 30830 - 17 bytes
            // 0     header lo   110
            // 1     header hi   120
            // 2     module ID
            // 3-10  sensor ROM code (8 bytes)
            // 11    temp lo
            // 12    temp hi
            // 13    user data 0
            // 14    user data 1
            // 15    sensors remaining
            // 16    CRC

            const int length = 17;
            if (data.Length < length) return;
            if (!ValidCrc(data, length)) return;

            byte moduleId = data[2];

            byte[] romCode = new byte[8];
            Array.Copy(data, 3, romCode, 0, 8);

            short rawTemp = (short)((data[12] << 8) | data[11]);
            byte userData0 = data[13];
            byte userData1 = data[14];
            byte remaining = data[15];

            var packet = new TemperaturePacket(moduleId, romCode, rawTemp,
                userData0, userData1, remaining, source);

            TemperatureReceived?.Invoke(this, packet);
        }

        private void ParseModuleDescription(byte[] data, IPEndPoint source)
        {
            // PGN 30831 - 22 bytes
            // 0     header lo   111
            // 1     header hi   120
            // 2     module ID
            // 3-8   module MAC (6 bytes)
            // 9-18  module name (10 bytes UTF-8)
            // 19-20 firmware version InoID (uint16 lo byte first)
            // 21    CRC

            const int length = 22;
            if (data.Length < length) return;
            if (!ValidCrc(data, length)) return;

            byte moduleId = data[2];

            byte[] mac = new byte[6];
            Array.Copy(data, 3, mac, 0, 6);

            string name = Encoding.UTF8.GetString(data, 9, 10).TrimEnd('\0');

            ushort firmwareVersion = (ushort)((data[20] << 8) | data[19]);

            var packet = new ModuleDescriptionPacket(moduleId, mac, name, firmwareVersion, source);

            ModuleDescriptionReceived?.Invoke(this, packet);
        }

        private static bool ValidCrc(byte[] data, int length)
        {
            byte sum = 0;
            for (int i = 0; i < length - 1; i++)
                sum += data[i];
            return sum == data[length - 1];
        }
    }
}
