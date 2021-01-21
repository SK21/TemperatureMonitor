using System;
using System.Diagnostics;

namespace TempMonitor.Classes
{
    public class PGN25110
    {
        //	Control Box to PC
        //PGN 25110
        //0	    Header Hi	98
        //1	    Header Lo	22
        //2	    Control Box ID
        //3     IP
        //4-9   Mac
        //10    RSSI
        //11-15 line markers

        private const byte cByteCount = 16;
        private const byte HeaderHi = 98;
        private const byte HeaderLo = 22;

        FormMain mf;
        private byte[] cData = new byte[cByteCount];
        byte boxIP;
        byte[] boxMac = new byte[6];
        string boxMacAddress;

        public event EventHandler<string> NewMessage;

        public PGN25110(FormMain CalledFrom)
        {
            mf = CalledFrom;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            try
            {
                if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
                {
                    for (int i = 0; i < cByteCount; i++)
                    {
                        cData[i] = Data[i];
                    }

                    boxIP = cData[3];
                    for (int i = 0; i < 6; i++)
                    {
                        boxMac[i] = cData[4 + i];
                    }
                    boxMacAddress = BitConverter.ToString(boxMac).Replace("-", ":");
                    PrintDebug();
                    SaveControlBoxData();
                    NewMessage?.Invoke(this, "");
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("PGN25110/ParseByteData: " + ex.Message);
            }
            return Result;
        }

        private void PrintDebug()
        {
            Debug.Print("");
            Debug.Print("IP " + boxIP.ToString());
            Debug.Print("MAC " + boxMacAddress);
            Debug.Print("RSSI " + (cData[10] - 100).ToString());

            Debug.Print("LineUDS " + cData[11].ToString());
            Debug.Print("LineASR " + cData[12].ToString());
            Debug.Print("LineSensors " + cData[13].ToString());
        }

        private void SaveControlBoxData()
        {
            try
            {
                // controlbox
                clsControlBox Box = new clsControlBox(mf);
                Box.Load(0, cData[2]);
                Box.BoxID = cData[2];
                Box.IPaddress = boxIP;
                Box.Mac = boxMacAddress;
                Box.Save();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("PGN25110/SaveControlBoxData: " + ex.Message);
            }
        }

    }
}
