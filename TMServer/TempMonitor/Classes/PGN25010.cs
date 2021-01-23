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
        //16    CRC

        private const byte cByteCount = 17;
        private const byte HeaderHi = 97;
        private const byte HeaderLo = 178;

        private byte[] cData = new byte[cByteCount];
        private FormMain mf;

        public PGN25010(FormMain CalledFrom)
        {
            mf = CalledFrom;
            cData[0] = HeaderHi;
            cData[1] = HeaderLo;
            cData[15] = 4;
            cData[cByteCount - 1] = mf.Tls.CRC8(cData, cByteCount - 1);
        }

        public void ReadSensors()
        {
            mf.UDP.SendUDPMessage(cData);
        }
    }
}
