using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempMonitor
{
    public class clsControlBox
    {
        // tblControlBoxes
        // cbID - record ID
        // cbNumber - Controlbox ID
        // cbDescription
        // cbUseSleep
        // cbUseDiagnostics
        // cbIPaddress
        // cbMac

        private byte cID;
        private byte cNumber;
        private string cDescription;
        private bool cUseSleep;
        private bool cUseDiagnostics;
        private byte cIPaddress;
        private string cMac;

        private FormMain mf;
        private bool NewRecord;

        public clsControlBox(FormMain CallingForm)
        {
            mf = CallingForm;
            cNumber = 0;
            cDescription = "";
            cUseSleep = false;
            NewRecord = true;
        }

        public byte ID { get { return cID; } }

        public byte BoxID
        {
            get { return cNumber; } 
            set
            {
                if (NewRecord & !UniqueID(value)) throw new ArgumentException("Duplicate ID: " + value.ToString());
                cNumber = value;
            }
        }

        public string Description
        {
            get { return cDescription; }
            set
            {
                if(value.Length>25)
                {
                    value = value.Substring(0, 25);
                }
                cDescription = value;
            }
        }

        public bool UseSleep { get { return cUseSleep; } set { cUseSleep = value; } }

        public bool UseDiagnostics { get { return cUseDiagnostics; } set { cUseDiagnostics = value; } }

        public byte IPaddress { get { return cIPaddress; } set { cIPaddress = value; } }

        public string Mac
        {
            get { return cMac; }
            set
            {
                if (value.Length > 17)
                {
                    value = value.Substring(0, 17);
                }
                cMac = value;
            }
        }

        public bool UniqueID(byte NewID)
        {
            bool Result;
            DAO.Recordset RS;
            string SQL = "select * from tblControlBoxes where cbNumber = " + NewID.ToString();
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            Result = RS.EOF;
            RS.Close();
            return Result;
        }

        public bool Load(byte ID = 0, byte ControlboxNumber = 0)
        {
            string SQL;
            if (ID == 0)
            {
                SQL = "select * from tblControlBoxes where cbNumber = " + ControlboxNumber.ToString();
            }
            else
            {
                SQL = "select * from tblControlBoxes where cbID = " + ID.ToString();
            }

            DAO.Recordset RS;
            RS = mf.Dbase.DB.OpenRecordset(SQL);

            if (RS.EOF)
            {
                RS.Close();
                return false;
            }
            else
            {
                cID = (byte)(RS.Fields["cbID"].Value ?? 0);
                cNumber = (byte)(RS.Fields["cbNumber"].Value ?? 0);
                cDescription = mf.Dbase.FieldToString(RS, "cbDescription");
                cUseSleep = mf.Dbase.FieldToBool(RS, "cbUseSleep");
                cUseDiagnostics = mf.Dbase.FieldToBool(RS, "cbUseDiagnostics");
                cIPaddress = (byte)mf.Dbase.FieldToInt(RS, "cbIPaddress");
                cMac = mf.Dbase.FieldToString(RS, "cbMac");

                NewRecord = false;
                RS.Close();
                return true;
            }
        }

        public void Save()
        {
            if (cNumber == 0) throw new ArgumentException("Controlbox ID not set.");
            DAO.Recordset RS;
            string SQL = "Select * from tblControlBoxes where cbID = " + cID.ToString();
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (NewRecord)
            {
                RS.AddNew();
            }
            else
            {
                RS.Edit();
            }
            RS.Fields["cbNumber"].Value = cNumber;
            RS.Fields["cbDescription"].Value = cDescription + " ";
            RS.Fields["cbUseSleep"].Value = cUseSleep;
            RS.Fields["cbUseDiagnostics"].Value = cUseDiagnostics;
            RS.Fields["cbIPaddress"].Value = cIPaddress;
            RS.Fields["cbMac"].Value = cMac + " ";

            RS.Update();
            if(NewRecord)
            {
                RS.set_Bookmark(RS.LastModified);
                cID = (byte)(RS.Fields["cbID"].Value ?? 0);
            }
            RS.Close();
            NewRecord = false;
        }
    }
}
