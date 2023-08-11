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
        private byte cID;
        private float cOffset;
        private byte cRecNum;
        private string cSensorAddress;
        private FormMain mf;
        private bool NewRecord;
        private byte cBinNum;
        private byte cCableNum;
        private byte cSensorNum;

        public clsSensor(FormMain CallingForm)
        {
            mf = CallingForm;
            SetRecNum();
            cSensorAddress = "";
            cControlBoxID = 0;
            cEnabled = true;
            cOffset = 0;
            NewRecord = true;
            cBinNum = 0;
            cCableNum = 0;
            cSensorNum = 0;
        }

        public byte BinNum
        {
            get
            {
                return cBinNum;
            }
            set
            {
                if (value > 255 | value < 0) throw new ArgumentException("Must be between 0 and 255.");
                cBinNum = value;
            }
        }

        public byte CableNum
        {
            get
            {
                return cCableNum;
            }
            set
            {
                if (value > 15 | value < 0) throw new ArgumentException("Must be between 0 and 15.");
                cCableNum = value;
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

        public byte SensorNum
        {
            get
            {
                return cSensorNum;
            }
            set
            {
                if (value > 15 | value < 0) throw new ArgumentException("Must be between 0 and 15.");
                cSensorNum = value;
            }
        }


        public int UserData
        {
            get
            {
                int Result = cBinNum << 8 | cCableNum << 4 | cSensorNum;
                return Result;
            }
            set
            {
                byte BinTmp = (byte)(value >> 8);
                byte CableTmp= (byte)((value & 0b0000000011110000) >> 4);
                byte SensorTmp = (byte)(value & 0b1111);
                if (CableTmp > 15 | SensorTmp > 15) throw new ArgumentException("Must be between 0 and 15");
                cBinNum = BinTmp;
                cCableNum = CableTmp;
                cSensorNum = SensorTmp;
            }
        }

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
                cID = (byte)(mf.Dbase.FieldToInt(RS, "senID"));
                cRecNum = (byte)(mf.Dbase.FieldToInt(RS, "senRecNum"));
                cSensorAddress = mf.Dbase.FieldToString(RS, "senAddress");
                cControlBoxID = (byte)(mf.Dbase.FieldToInt(RS, "senControlBoxID"));
                cEnabled = mf.Dbase.FieldToBool(RS, "senEnabled");
                cOffset = mf.Dbase.FieldToFloat(RS, "senOffSet");
                cBinNum = (byte)(mf.Dbase.FieldToInt(RS, "senBinNumber"));
                cCableNum = (byte)(mf.Dbase.FieldToInt(RS, "senCableNumber"));
                cSensorNum = (byte)(mf.Dbase.FieldToInt(RS, "senSensorNumber"));
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
                result += "    Cable: " + CableNum.ToString();
                result += "    Sensor: " + SensorNum.ToString();
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
            RS.Fields["senControlBoxID"].Value = cControlBoxID;
            RS.Fields["senEnabled"].Value = cEnabled;
            RS.Fields["senOffset"].Value = cOffset;
            RS.Fields["senLastTemp"].Value = LastTemp();
            RS.Fields["senLastTime"].Value = LastTime();
            RS.Fields["senBinNumber"].Value = cBinNum;
            RS.Fields["senCableNumber"].Value = cCableNum;
            RS.Fields["senSensorNumber"].Value = cSensorNum;
            RS.Update();
            if (NewRecord)
            {
                // update with new autoincrement ID
                RS.set_Bookmark(RS.LastModified);
                cID = (byte)(mf.Dbase.FieldToInt(RS, "senID"));
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
                cRecNum = (byte)(mf.Dbase.FieldToInt(RS, "senRecNum"));
                cRecNum += 1;
            }
            RS.Close();
        }
    }
}