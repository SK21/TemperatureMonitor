using System;
using System.Collections.Generic;

namespace TempMonitor
{
    internal class clsStorages
    {
        public IList<clsStorage> Items;
        private FormMain mf;
        private List<clsStorage> Storages = new List<clsStorage>();

        public clsStorages(FormMain CallingForm)
        {
            mf = CallingForm;
            Items = Storages.AsReadOnly();
        }

        public clsStorage Add(int StorNum = 0, string Description = "")
        {
            if (StorNum == 0)
            {
                int Result = 0;
                // new record, increment highest value
                foreach (clsStorage Stor in Storages)
                {
                    if (Stor.Number > Result) Result = Stor.Number;
                }
                StorNum = Result + 1;
            }
            else
            {
                // check if exists
                foreach (clsStorage Stor in Storages)
                {
                    if (Stor.Number == StorNum) return Stor;
                }
            }

            // new record
            Storages.Add(new clsStorage(mf));
            clsStorage StorNew = Storages[Storages.Count - 1];
            StorNew.Number = StorNum;
            StorNew.Description = Description;
            return StorNew;
        }

        public int Count()
        {
            return Storages.Count;
        }

        public void Delete(int ItemID)
        {
            int IDX = ListID(ItemID);
            if (IDX != -1)
            {
                // remove from list
                Storages.RemoveAt(IDX);

                // remove from database
                DAO.Recordset RS;
                string SQL = "Select * from tblStorage where storID = " + ItemID.ToString();
                RS = mf.Dbase.DB.OpenRecordset(SQL);
                if (!RS.EOF) RS.Delete();
                RS.Close();
            }
        }

        public clsStorage Item(int ItemID, bool AddNew = false)
        {
            int IDX = ListID(ItemID);
            if (IDX == -1)
            {
                if (AddNew)
                {
                    return Add();
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            return Storages[IDX];
        }

        public void Load()
        {
            Storages.Clear();

            DAO.Recordset RS;
            string SQL = "select * from tblStorage order by storRecNum";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            while (!RS.EOF)
            {
                clsStorage Stor = new clsStorage(mf);
                Stor.Load((short)RS.Fields["storID"].Value);
                Storages.Add(Stor);
                RS.MoveNext();
            }
            RS.Close();
        }

        private int ListID(int ItemID)
        {
            for (int i = 0; i < Storages.Count; i++)
            {
                if (Storages[i].ID == ItemID) return i;
            }
            return -1;
        }
    }
}