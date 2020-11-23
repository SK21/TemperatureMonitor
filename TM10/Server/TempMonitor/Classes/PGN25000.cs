namespace TempMonitor.Classes
{
    public class PGN25000
    {
        //	PC to Control Box
        //		PGN 25000
        //0		Header Hi	97
        //1		Header Lo	168
        //2		Control Box ID
        //3-10	Sensor Address bytes 0-8
        //11	User Data Hi
        //12	User Data Lo
        //13	-
        //14	-
        //15		Command byte
        //			- 1 all sensors report
        //			- 2 specific sensor report
        //			- 3 set sensor userdata
        //			- 4 read sensors

        private const byte cByteCount = 16;
        private const byte HeaderHi = 97;
        private const byte HeaderLo = 168;

        private byte[] cData = new byte[cByteCount];
        private FormMain mf;

        public PGN25000(FormMain CalledFrom)
        {
            mf = CalledFrom;
            cData[0] = HeaderHi;
            cData[1] = HeaderLo;
        }

        public void AllSensorsReport(byte ControlBoxID)
        {
            cData[2] = ControlBoxID;
            cData[15] = 1;
            Send();
        }

        public void ReadSensors()
        {
            cData[15] = 4;
            Send();
        }

        public void SetUserData(byte ControlBoxID, byte[] Addr, int UserData)
        {
            cData[2] = ControlBoxID;
            cData[15] = 3;
            for (int i = 0; i < 8; i++)
            {
                cData[i + 3] = Addr[i];
            }
            cData[11] = (byte)(UserData >> 8);
            cData[12] = (byte)UserData;
            Send();
        }

        public void SpecificSensorReport(byte ControlBoxID, byte[] Addr)
        {
            cData[2] = ControlBoxID;
            cData[15] = 2;
            for (int i = 0; i < 8; i++)
            {
                cData[i + 3] = Addr[i];
            }
            Send();
        }

        private void Send()
        {
            mf.UDP.SendUDPMessage(cData);
        }
    }
}