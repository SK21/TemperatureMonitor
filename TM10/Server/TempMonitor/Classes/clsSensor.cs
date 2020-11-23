using System;

namespace TempMonitor
{
    public class clsSensor
    {
        // user data saved on the sensor:
        // 16 bit data
        // 11111111  1111  1111
        // bin       cable sensor
        // 0-255     0-15  0-15

        private byte[] AddressBytes = new byte[8];
        private byte cControlBoxID;
        private bool cEnabled;
        private short cID;
        private float cOffset;
        private short cRecNum;
        private string cSensorAddress;
        private int cUserData;
        private FormMain mf;
        private bool NewRecord;

        public clsSensor(FormMain CallingForm)
        {
            mf = CallingForm;
            SetRecNum();
            cSensorAddress = "";
            cUserData = 0;
            cControlBoxID = 0;
            cEnabled = true;
            cOffset = 0;
            NewRecord = true;
        }

        public short BinNum
        {
            get
            {
                return (byte)(cUserData >> 8); // get left 8 bits
            }
            set
            {
                if (value > 255 | value < 0) throw new ArgumentException("Must be between 0 and 255.");
                cUserData = (UInt16)(cUserData & 0b0000000011111111);
                cUserData = (UInt16)(cUserData | value << 8);
            }
        }

        public short CableID
        {
            get
            {
                return (short)((cUserData & 0b0000000011110000) >> 4);
            }
            set
            {
                if (value > 15 | value < 0) throw new ArgumentException("Must be between 0 and 15.");
                cUserData = (UInt16)(cUserData & 0b1111111100001111);
                cUserData = (UInt16)(cUserData | (value << 4));
            }
        }

        public byte ControlBoxID { get { return cControlBoxID; } set { cControlBoxID = value; } }

        public bool Enabled { get { return cEnabled; } set { cEnabled = value; } }

        public short ID { get { return cID; } }

        public bool IsNew { get { return NewRecord; } }

        public float OffSet
        {
            get { return cOffset; }
            set
            {
                if (value < -50 | value > 50) throw new ArgumentException("Must be between -50 and 50.");
                cOffset = value;
            }
        }

        public short RecNum { get { return cRecNum; } }

        public string SensorAddress
        {
            get
            {
                return cSensorAddress;
            }
            set
            {
                // expects address hex representation ex: "28 29 91 3C 07 00 00 64"

                string[] Parts;
                byte PartVal;
                bool result = false;
                Parts = value.Split(separator: ' ');

                if (Parts.Length != 8) throw new ArgumentException("Invalid address.");

                for (int i = 0; i < 8; i++)
                {
                    result = byte.TryParse(Parts[i], System.Globalization.NumberStyles.HexNumber, null, out PartVal);
                    if (result = false | PartVal > 255)
                    {
                        throw new OverflowException();
                    }
                }
                cSensorAddress = value;
            }
        }

        public short SensorID
        {
            get
            {
                return (short)(cUserData & 0b1111);
            }
            set
            {
                if (value > 15 | value < 0) throw new ArgumentException("Must be between 0 and 15.");
                cUserData = (UInt16)(cUserData & 0b1111111111110000);
                cUserData = (UInt16)(cUserData | (UInt16)value);
            }
        }

        public int UserData { get { return cUserData; } set { cUserData = value; } }

        public string BinDescription(bool ShortDescription = false)
        {
            string result = "";
            if (ShortDescription)
            {
                result = BinNum.ToString();
            }
            else
            {
                clsStorage Bin = new clsStorage(mf);
                if (BinNum > 0)
                {
                    Bin.Load(StorNum: BinNum);
                    result = Bin.Number.ToString() + "  " + Bin.Description;
                }
            }
            return result;
        }

        public string ConvertAddressBytes(byte[] NewBytes)
        {
            if (NewBytes.Length == 8)
            {
                // converts array of bytes to hex representation ex: "28 29 91 3C 07 00 00 64"
                return BitConverter.ToString(NewBytes).Replace("-", " ");
            }
            else
            {
                return "";
            }
        }

        public byte[] ConvertAddressString(string Adr)
        {
            // converts address string to a byte array
            // expects address hex representation ex: "28 29 91 3C 07 00 00 64"

            string[] Parts;
            byte[] Result = new byte[8];

            Parts = Adr.Split(separator: ' ');
            if (Parts.Length == 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (!byte.TryParse(Parts[i], System.Globalization.NumberStyles.HexNumber, null, out Result[i]))
                    {
                        // error in parsing
                        for (int j = 0; j < 8; j++)
                        {
                            Result[j] = 0;
                        }
                        break;
                    }
                }
            }
            return Result;
        }

        public byte[] GetAddressBytes()
        {
            return AddressBytes;
        }

        public float LastTemp()
        {
            // look up in tblRecs using LastRecID
            DAO.Recordset RS;
            string SQL = "Select Top 1 tblRecords.recTemp,tblRecords.recTimeStamp";
            SQL += " From tblRecords";
            SQL += " Where recSenID = " + cID;
            SQL += " Order By recTimeStamp DESC";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (RS.EOF)
            {
                RS.Close();
                return -127;
            }
            else
            {
                //RS.Close();
                return (float)RS.Fields["recTemp"].Value;
            }
        }

        public DateTime LastTime()
        {
            // look up in tblRecs using LastRecID
            DAO.Recordset RS;
            string SQL = "Select Top 1 tblRecords.recTemp,tblRecords.recTimeStamp";
            SQL += " From tblRecords";
            SQL += " Where recSenID = " + cID;
            SQL += " Order By recTimeStamp DESC";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (RS.EOF)
            {
                RS.Close();
                return DateTime.Parse("01/01/1900");
            }
            else
            {
                //RS.Close();
                return RS.Fields["recTimeStamp"].Value;
            }
        }

        public bool Load(short ID = -1, short ControlBoxID = -1, string SensorAddress = "", short UserData = -1, byte[] SensorByteAddress = null)
        {
            // returns true if record loaded, false is new record

            DAO.Recordset RS;
            string SQL;
            if (ControlBoxID > -1)
            {
                SQL = "Select * from tblSensors where senControlBoxID = " + ControlBoxID;
            }
            else if (SensorAddress != "")
            {
                SQL = "Select * from tblSensors where senAddress = '" + SensorAddress + "'";
            }
            else if (UserData > -1)
            {
                SQL = "Select * from tblSensors where senUserData = " + UserData;
            }
            else if (SensorByteAddress != null)
            {
                SQL = "Select * from tblSensors where senAddress = '" + ConvertAddressBytes(SensorByteAddress) + "'";
            }
            else
            {
                SQL = "Select * from tblSensors where senID = " + ID;
            }

            RS = mf.Dbase.DB.OpenRecordset(SQL);

            if (RS.EOF)
            {
                RS.Close();
                return false;
            }
            else
            {
                cID = (short)(RS.Fields["SenID"].Value ?? 0);    // returns 0 if null
                cRecNum = (short)(RS.Fields["senRecNum"].Value ?? 0);
                if (Convert.IsDBNull(RS.Fields["senAddress"].Value)) cSensorAddress = ""; else cSensorAddress = RS.Fields["senAddress"].Value;
                cUserData = (UInt16)(RS.Fields["senUserData"].Value ?? 0);
                cControlBoxID = (byte)(RS.Fields["senControlBoxID"].Value ?? 0);
                cEnabled = RS.Fields["senEnabled"].Value ?? 0;
                cOffset = (float)(RS.Fields["senOffSet"].Value ?? 0);
                NewRecord = false;
                RS.Close();

                // build address bytes
                AddressBytes = ConvertAddressString(cSensorAddress);

                return true;
            }
        }

        public string Name()
        {
            string result;
            if (BinNum == 0)
            {
                result = "ID: " + SensorAddress;
            }
            else
            {
                result = "    Bin: " + BinNum.ToString();
                result += "    Cable: " + CableID.ToString();
                result += "    Sensor: " + SensorID.ToString();
            }
            return result;
        }

        public void Save()
        {
            if (ValidationErrors() != "") throw new ArgumentException(ValidationErrors());

            DAO.Recordset RS;
            string SQL;
            SQL = "Select * from tblSensors where senID =" + cID;
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (NewRecord)
            {
                RS.AddNew();
            }
            else
            {
                RS.Edit();
            }
            RS.Fields["senRecNum"].Value = cRecNum;
            RS.Fields["senAddress"].Value = cSensorAddress;
            RS.Fields["senUserData"].Value = cUserData;
            RS.Fields["senControlBoxID"].Value = cControlBoxID;
            RS.Fields["senEnabled"].Value = cEnabled;
            RS.Fields["senOffset"].Value = cOffset;
            RS.Fields["senLastTemp"].Value = LastTemp();
            RS.Fields["senLastTime"].Value = LastTime();
            RS.Update();
            if (NewRecord)
            {
                // update with new autoincrement ID
                RS.set_Bookmark(RS.LastModified);
                cID = (short)(RS.Fields["senID"].Value ?? 0);
            }
            RS.Close();
            NewRecord = false;
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

        public string ValidationErrors()
        {
            string result = "";
            if (cSensorAddress == "") result += "Invalid sensor address.";
            if (DuplicateSensor()) result += "Duplicate sensor.";
            return result;
        }

        private bool DuplicateSensor()
        {
            bool result = false;
            if (NewRecord)
            {
                DAO.Recordset RS;
                string SQL;
                SQL = "Select * from tblSensors where senAddress = '" + cSensorAddress + "'";
                RS = mf.Dbase.DB.OpenRecordset(SQL);
                result = (!RS.EOF);
                RS.Close();
            }
            return result;
        }

        private void SetRecNum()
        {
            DAO.Recordset RS;
            string SQL = "Select * from tblSensors order by senRecNum";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (RS.EOF)
            {
                cRecNum = 1;
            }
            else
            {
                RS.MoveLast();
                cRecNum = (short)(RS.Fields["senRecNum"].Value ?? 0);
                cRecNum += 1;
            }
            RS.Close();
        }
    }
}