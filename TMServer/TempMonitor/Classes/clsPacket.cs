using System;

namespace TempMonitor
{
    public class clsPacket
    {
        // commands:
        // 1    all sensors report
        // 2    specific sensor report
        // 3    set sensor userdata
        // 4    read sensors

        private byte[] AddressBytes = new byte[8];
        private PacketType cCommandID = PacketType.AllSensorsReport;
        private byte cControlBoxID = 0;
        private string cSensorAddress;
        private float cTemperature;
        private DateTime cTimeStamp = DateTime.Now;
        private int cUserData;

        public PacketType CommandID
        {
            get { return cCommandID; }

            set { cCommandID = value; }
        }

        public byte ControlBoxID { get { return cControlBoxID; } set { cControlBoxID = value; } }

        public string SensorAddress { get { return cSensorAddress; } }

        public float Temperature { get { return cTemperature; } set { cTemperature = value; } }

        public DateTime TimeStamp { get { return cTimeStamp; } set { cTimeStamp = value; } }

        public int UserData { get { return cUserData; } set { cUserData = value; } }

        public byte[] GetAddressBytes()
        {
            return AddressBytes;
        }

        public void SetAddressBytes(byte[] NewBytes)
        {
            if (NewBytes.Length == 8)
            {
                AddressBytes = NewBytes;
                // converts array of bytes to hex representation ex: "28 29 91 3C 07 00 00 64"
                cSensorAddress = BitConverter.ToString(NewBytes).Replace("-", " ");
            }
        }

        public void SetTemperatureBytes(byte B1, byte B2)
        {
            // expects 2 bytes that are 10 times actual temperature
            cTemperature = (float)((B1 << 8 | B2) / 10);
        }

        public void SetUserDataBytes(byte B1, byte B2)
        {
            cUserData = (short)(B1 << 8 | B2);
        }
    }
}