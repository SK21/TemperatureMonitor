﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempMonitor
{
    public class PGN25020
    {
        // pc to controlbox, settings
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
        // 9    ControlBox count
        // 10   TimeSlot length in minutes
        // 11   Send Diagnostics back
        // 12   restart 
        // 16   CRC

        private const byte cByteCount = 17;
        private const byte HeaderHi = 97;
        private const byte HeaderLo = 188;

        private byte[] cData = new byte[cByteCount];
        private FormMain mf;
        private bool ChangedID = false;
        private bool cSendDiag = false;
        private bool cRestart = false;

        public PGN25020(FormMain CalledFrom)
        {
            mf = CalledFrom;
            cData[0] = HeaderHi;
            cData[1] = HeaderLo;
            cData[10] = 5;  // TimeSlot
        }

        public byte ControlBoxNumber
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

        public byte ControlBoxCount
        {
            get { return cData[9]; }
            set { cData[9] = value; }
        }

        public byte TimeSlot
        {
            get { return cData[10]; }
            set { cData[10] = value; }
        }

        public bool SendDiagnostics
        {
            get { return cSendDiag; }
            set
            {
                cSendDiag = value;
                if (cSendDiag)
                {
                    cData[11] = 1;
                }
                else
                {
                    cData[11] = 0;
                }
            }
        }

        public bool Restart
        {
            get { return cRestart; }
            set
            {
                cRestart = value;
                if(cRestart)
                {
                    cData[12] = 1;
                }
                else
                {
                    cData[12] = 0;
                }
            }
        }

        public void Send()
        {
            if (!ChangedID) cData[6] = cData[2];    // make sure either new ID or current ID
            ChangedID = false;
            CurrentTime = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            cData[cByteCount - 1] = mf.Tls.CRC8(cData, cByteCount - 1);

            mf.UDP.SendUDPMessage(cData);

            // reset values
            //SendDiagnostics = false;
            Restart = false;
        }
    }
}
