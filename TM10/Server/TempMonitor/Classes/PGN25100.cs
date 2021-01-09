using System;
using System.Diagnostics;

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
        //15	0 - data remaining, 1 - finished, 2 - no sensors, 3 - GoToSleep

        private const byte cByteCount = 16;
        private const byte HeaderHi = 98;
        private const byte HeaderLo = 12;

        private byte[] cData = new byte[cByteCount];
        private string cSensorName;
        private FormMain mf;

        public bool ShowOnChipData = false; // either on chip or from database

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

        public byte ControlBoxNumber()
        {
            return cData[2];
        }

        public string SensorMessage()
        {
            string Mess = "CBX: " + cData[2].ToString() + "  ";
            if (ShowOnChipData)
            {
                Mess += "    Bin: " + mf.Tls.BinNumFromUserData(UserData()).ToString();
                Mess += "    Cable: " + mf.Tls.CableNumFromUserData(UserData()).ToString();
                Mess += "    Sensor: " + mf.Tls.SensorNumFromUserData(UserData()).ToString();
            }
            else
            {
                // database record
                Mess += cSensorName;
            }
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

                switch (cData[15])
                {
                    case 0: // more sensors remaining
                    case 1: // last sensor
                        Result = true;
                        SaveSensorData();
                        NewMessage?.Invoke(this, SensorMessage());
                        break;
                    case 2:
                        // no sensors
                        NewMessage?.Invoke(this, "CBX: " + cData[2].ToString() + "      no sensors available.");
                        break;
                    case 3:
                        // controlbox went to sleep
                        NewMessage?.Invoke(this, "CBX: " + cData[2].ToString() + "      went to sleep.");
                        break;
                    case 4:
                        // diagnostics
                        Debug.Print("");
                        Debug.Print("LineUDS " + cData[3].ToString());
                        Debug.Print("LineASR " + cData[4].ToString());
                        Debug.Print("RSSI " + (cData[5] - 100).ToString());
                        Debug.Print("LineSensors " + cData[6].ToString());
                        break;
                }
                SaveControlBoxData();
            }
            return Result;
        }

        public void SaveSensorData()
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

        private void SaveControlBoxData()
        {
            try
            {
                // controlbox
                clsControlBox Box = new clsControlBox(mf);
                if (!Box.Load(0, cData[2]))
                {
                    // new record
                    Box.BoxID = cData[2];
                    Box.Save();
                }
            }
            catch (Exception ex)
            {
                mf.WriteEvent(ex.Message);
                mf.Tls.WriteErrorLog("PGN25100/SaveControlBox: " + ex.Message);
            }
        }

        public string StringAddress()
        {
            return mf.Tls.HexAddressFromBytes(Address());
        }

        public float Temperature()
        {
            return FromTwos(cData[13], cData[14]);
        }

        public int UserData()
        {
            return cData[11] << 8 | cData[12];
        }

        public bool Finished()
        {
            return Convert.ToBoolean(cData[15]);
        }

        float FromTwos(byte MSB, byte LSB)
        {
            float r;
            UInt16 t = 0;
            UInt16 s = (ushort)((MSB << 8) | LSB);
            if ((s & 32768) == 32768)
            {
                // negative number
                t = (ushort)(~s + 1);   // complement + 1
                r = (float)((t * -1.0));// multiply by -1 to give two's complement
                r = (float)(r / 16.0);  // convert one-wire temperatures
            }
            else
            {
                // positive number
                r = (float)(s / 16.0);
            }
            return r;
        }
    }
}