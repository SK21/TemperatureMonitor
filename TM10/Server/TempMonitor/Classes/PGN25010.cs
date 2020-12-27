using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TempMonitor
{
    public class PGN25010
    {
        // read sensors
        // PGN25010
        //0     Header Hi   97
        //1     Header Lo   178
        //2-14  -
        //15    4

        private const byte cByteCount = 16;
        private const byte HeaderHi = 97;
        private const byte HeaderLo = 178;

        private byte[] cData = new byte[cByteCount];
        private FormMain mf;

        public PGN25010(FormMain CalledFrom)
        {
            mf = CalledFrom;
            cData[0] = HeaderHi;
            cData[1] = HeaderLo;
            //for (int i = 2; i < 15; i++)
            //{
            //    cData[i] = 0;
            //}
            cData[15] = 4;
        }

        public void ReadSensors()
        {
            mf.UDP.SendUDPMessage(cData);
            Debug.WriteLine("Sent ReadSensors");
            Debug.WriteLine("");
        }
    }
}
