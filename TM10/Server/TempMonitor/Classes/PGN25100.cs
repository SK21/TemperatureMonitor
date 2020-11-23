using System;

namespace TempMonitor.Classes
{
    public class PGN25100
    {
        //	Control Box to PC
        //PGN 25100
        //0	Header Hi	98
        //1	Header Lo	12
        //2	Control Box ID
        //3-10	Sensor Address bytes 0-8
        //11	User Data Hi
        //12	User Data Lo
        //13	Temp Hi
        //14	Temp Lo
        //15	-

        private const byte cByteCount = 16;
        private const byte HeaderHi = 98;
        private const byte HeaderLo = 12;

        private byte[] cData = new byte[cByteCount];
        private string cSensorName;
        private FormMain mf;

        public PGN25100(FormMain CalledFrom)
        {
            mf = CalledFrom;
        }

        public event EventHandler<string> NewMessage;

        public byte[] Address()
        {
            byte[] addr = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                addr[i] = cData[i + 3];
            }
            return addr;
        }

        public byte ControlBoxID()
        {
            return cData[2];
        }

        public string Message()
        {
            string Mess = cSensorName;
            Mess += "   Temp: " + Temperature().ToString("N1");
            return Mess;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                for (int i = 0; i < cByteCount; i++)
                {
                    cData[i] = Data[i];
                }
                Result = true;
                SaveData();
                NewMessage?.Invoke(this, Message());
            }
            return Result;
        }

        public void SaveData()
        {
            try
            {
                // sensor
                clsSensor Sen = new clsSensor(mf);
                if (!Sen.Load(SensorAddress: StringAddress()))
                {
                    // new record
                    Sen.SetAddressBytes(Address());
                    Sen.Enabled = true;
                    Sen.OffSet = 0;
                }
                //Sen.UserData = (short)UserData();		// should be manually saved
                Sen.ControlBoxID = cData[2];
                Sen.Save();
                cSensorName = Sen.Name();

                if (mf.RecordData)
                {
                    // temperatures
                    clsRecord Rec = new clsRecord(mf);
                    Rec.SenID = Sen.ID;
                    Rec.Temperature = Temperature();
                    Rec.Save();
                }
            }
            catch (Exception ex)
            {
                mf.WriteEvent(ex.Message);
                mf.Tls.WriteErrorLog("PGN25100: " + ex.Message);
            }
        }

        public string StringAddress()
        {
            return mf.Tls.HexAddressFromBytes(Address());
        }

        public float Temperature()
        {
            return (float)((cData[13] << 8 | cData[14]) / 10.0);
        }

        public int UserData()
        {
            return cData[11] << 8 | cData[12];
        }
    }
}