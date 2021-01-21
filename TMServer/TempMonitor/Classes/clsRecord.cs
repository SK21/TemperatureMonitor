using System;

namespace TempMonitor
{
    internal class clsRecord
    {
        // recID
        // recRecNum
        // recSenID
        // recTemp
        // recTimeStamp

        private short cID;
        private short cRecNum;
        private short cSenID;
        private float cTemp;
        private DateTime cTimeStamp;

        private FormMain mf;
        private bool NewRecord;

        public clsRecord(FormMain CallingForm)
        {
            mf = CallingForm;
            SetRecNum();
            cSenID = 0;
            cTemp = 0;
            NewRecord = true;
        }

        public short ID { get { return cID; } }

        public short RecNum { get { return cRecNum; } }

        public short SenID { get { return cSenID; } set { cSenID = value; } }

        public float Temperature
        {
            get { return cTemp; }

            set
            {
                if (value < 100 & value > -200)
                {
                    cTimeStamp = DateTime.Now;
                    cTemp = value;
                }
            }
        }

        public DateTime TimeStamp { get { return cTimeStamp; } }

        public bool IsValid()
        {
            return (cSenID > 0) & (cTemp < 100) & (cTemp > -200);
        }

        public void Load(short recID = 0, short SenID = 0)
        {
            DAO.Recordset RS;
            string SQL = "select * from tblRecords";
            if (SenID > 0)
            {
                SQL += " where recSenID = " + SenID.ToString();
            }
            else
            {
                SQL += " where recID = " + recID.ToString();
            }
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (!RS.EOF)
            {
                cID = RS.Fields["recID"].Value ?? 0;
                cRecNum = RS.Fields["recRecNum"].Value ?? 0;
                cSenID = RS.Fields["recSenID"].Value ?? 0;
                cTemp = RS.Fields["recTemp"].Value ?? 0;
                cTimeStamp = RS.Fields["recTimeStamp"].Value ?? DateTime.Parse("01/01/1900");
                NewRecord = false;
            }
            RS.Close();
        }

        public bool Save()
        {
            if (IsValid())
            {
                DAO.Recordset RS;
                string SQL;
                SQL = "Select * from tblRecords where recID =" + cID;
                RS = mf.Dbase.DB.OpenRecordset(SQL);
                if (NewRecord)
                {
                    RS.AddNew();
                }
                else
                {
                    RS.Edit();
                }
                RS.Fields["recRecNum"].Value = cRecNum;
                RS.Fields["recSenID"].Value = cSenID;
                RS.Fields["recTemp"].Value = cTemp;
                RS.Fields["recTimeStamp"].Value = cTimeStamp;
                RS.Update();
                if (NewRecord)
                {
                    // update with new autoincrement ID
                    RS.set_Bookmark(RS.LastModified);
                    cID = (short)(RS.Fields["recID"].Value ?? 0);
                }
                RS.Close();
                NewRecord = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetRecNum()
        {
            DAO.Recordset RS;
            string SQL = "Select * from tblRecords order by recRecNum";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (RS.EOF)
            {
                cRecNum = 1;
            }
            else
            {
                RS.MoveLast();
                cRecNum = (short)(RS.Fields["recRecNum"].Value ?? 0);
                cRecNum += 1;
            }
            RS.Close();
        }
    }
}