namespace TempMonitor
{
    internal class clsStorage
    {
        // ID
        // record Number
        // storage Number
        // description

        private string cDescription;
        private short cID;
        private int cNumber;
        private short cRecNum;
        private FormMain mf;
        private bool NewRecord = true;

        public clsStorage(FormMain CallingForm)
        {
            mf = CallingForm;
            SetRecNum();
        }

        public string Description { get { return cDescription; } set { cDescription = value; } }

        public short ID { get { return cID; } }

        public int Number
        {
            get { return cNumber; }

            set
            {
                // check if unique
                DAO.Recordset RS;
                string SQL = "Select * from tblStorage where storNum = " + value.ToString();
                RS = mf.Dbase.DB.OpenRecordset(SQL);
                if (RS.EOF)
                {
                    cNumber = value;
                }
                RS.Close();
            }
        }

        public short RecNum { get { return cRecNum; } }

        public void Load(short recID = 0, short StorNum = 0)
        {
            string SQL;
            if (recID == 0)
            {
                SQL = "select * from tblStorage where storNum = " + StorNum.ToString();
            }
            else
            {
                SQL = "select * from tblStorage where storID = " + recID.ToString();
            }
            DAO.Recordset RS;
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (!RS.EOF)
            {
                cID = (short)(RS.Fields["storID"].Value ?? 0);
                cRecNum = (short)(RS.Fields["storRecNum"].Value ?? 0);
                cNumber = (int)(RS.Fields["storNum"].Value ?? 0);
                cDescription = (string)(RS.Fields["storDescription"].Value ?? "");
            }
            RS.Close();
        }

        public void Save()
        {
            DAO.Recordset RS;
            string SQL = "Select * from tblStorage where storID = " + cID;
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (NewRecord)
            {
                RS.AddNew();
            }
            else
            {
                RS.Edit();
            }
            RS.Fields["storRecNum"].Value = cRecNum;
            RS.Fields["storNum"].Value = cNumber;
            RS.Fields["storDescription"].Value = cDescription;
            RS.Update();
            if (NewRecord)
            {
                RS.set_Bookmark(RS.LastModified);
                cID = (short)(RS.Fields["storID"].Value ?? 0);
            }
            RS.Close();
            NewRecord = false;
        }

        private void SetRecNum()
        {
            DAO.Recordset RS;
            string SQL = "Select * from tblStorage order by storRecNum";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (RS.EOF)
            {
                cRecNum = 1;
            }
            else
            {
                RS.MoveLast();
                cRecNum = (short)(RS.Fields["storRecNum"].Value ?? 0);
                cRecNum += 1;
            }
            RS.Close();
        }
    }
}