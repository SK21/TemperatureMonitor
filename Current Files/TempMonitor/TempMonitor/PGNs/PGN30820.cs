using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempMonitor.Forms;

namespace TempMonitor.PGNs
{
    public class PGN30820
    {
        //PGN30820, temps from module to app
        //0     HeaderLo    100
        //1     HeaderHi    120
        //2-13  module ID
        //14    user data lo
        //15    user data hi
        //16    temp lo
        //17    temp hi
        //18-25 sensor address
        //26    crc

        private const byte cByteCount = 27;
        private const byte HeaderHi = 120;
        private const byte HeaderLo = 100;
        private string cModuleID;
        private byte[] cSensorAddress = new byte[8];
        private double cTemp;
        private UInt16 cUserData;
        private frmMain mf;

        public PGN30820(frmMain CF)
        {
            mf = CF;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            byte[] ID = new byte[12];

            if (Data[1] == HeaderHi && Data[0] == HeaderLo &&
                Data.Length >= cByteCount && mf.Tls.GoodCRC(Data))
            {
                Array.Copy(Data, 2, ID, 0, 12);
                cModuleID = Encoding.Unicode.GetString(ID);

                cUserData = (ushort)(Data[14] | Data[15] << 8);
                cTemp = (Data[16] | Data[17] << 8) / 10.0;

                Array.Copy(Data, 18, cSensorAddress, 0, 8);
                SaveData();
                Result = true;
            }

            return Result;
        }

        private void SaveData()
        {
        }
    }
}