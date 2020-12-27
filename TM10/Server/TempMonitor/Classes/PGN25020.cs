using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempMonitor
{
    public class PGN25020
    {
        // send controlbox data
        // PGN25020
        // 0    Header Hi   97
        // 1    Header Lo   188
        // 2	controlbox ID
        // 3    use sleep 0-no, 1-yes
        // 4    sleep interval Hi
        // 5    sleep interval Lo
        // 6    new controlbox ID
        // 7    Current Time Hi
        // 8    Current Time Lo
        // 9-15 -

        private const byte cByteCount = 16;
        private const byte HeaderHi = 97;
        private const byte HeaderLo = 188;

        private byte[] cData = new byte[cByteCount];
        private FormMain mf;
        private bool ChangedID = false;

        public PGN25020(FormMain CalledFrom)
        {
            mf = CalledFrom;
            cData[0] = HeaderHi;
            cData[1] = HeaderLo;
        }

        public byte ControlBoxID
        {
            get { return cData[2]; }
            set { cData[2] = value; }
        }

        public bool UseSleep
        {
            get { return Convert.ToBoolean(cData[3]); }
            set { cData[3] = Convert.ToByte(value); }
        }

        public int SleepInterval
        {
            get { return cData[4] << 8 | cData[5]; }
            set
            {
                if (value < 0) value = 0;
                if (value > 1439) value = 1439; // max one day, 0-1439 minutes
                cData[4] = (byte)(value >> 8);
                cData[5] = (byte)value;
            }
        }

        public byte NewControlBoxID
        {
            get { return cData[6]; }
            set
            {
                clsControlBox Box = new clsControlBox(mf);
                if(!Box.UniqueID(value)) throw new ArgumentException("Duplicate ID: "+value.ToString());

                cData[6] = value;
                ChangedID = true;
            }
        }

        public int CurrentTime
        {
            get { return cData[7] << 8 | cData[8]; }
            set
            {
                cData[7] = (byte)(value >> 8);
                cData[8] = (byte)value;
            }
        }

        public void Send()
        {
            if (!ChangedID) cData[6] = cData[2];    // make sure either new ID or current ID
            ChangedID = false;
            CurrentTime = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            mf.UDP.SendUDPMessage(cData);
        }
    }
}
