using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempMonitor
{
    class clsControlBoxes
    {
        public IList<clsControlBox> Items;
        private List<clsControlBox> cControlBoxes = new List<clsControlBox>();
        private FormMain mf;

        public clsControlBoxes(FormMain CallingForm)
        {
            mf = CallingForm;
            Items = cControlBoxes.AsReadOnly();
        }

        public clsControlBox Add(clsControlBox NewControlBox)
        {
            cControlBoxes.Add(new clsControlBox(mf));
            clsControlBox Box = cControlBoxes[cControlBoxes.Count - 1];
            Box.BoxID = NewControlBox.BoxID;
            Box.Description = NewControlBox.Description;
            Box.UseSleep = NewControlBox.UseSleep;
            return Box;
        }

        public clsControlBox Add()
        {
            cControlBoxes.Add(new clsControlBox(mf));
            clsControlBox Box = cControlBoxes[cControlBoxes.Count - 1];
            return Box;
        }

        public int Count()
        {
            return cControlBoxes.Count;
        }

        private int ListID(byte ID)
        {
            for(int i=0;i<cControlBoxes.Count;i++)
            {
                if (cControlBoxes[i].ID == ID) return i;
            }
            return -1;
        }

        public void Delete(byte ID)
        {
            int IDX = ListID(ID);
            if (IDX == -1) throw new IndexOutOfRangeException();

            // remove from list
            cControlBoxes.RemoveAt(IDX);

            // remove from database
            DAO.Recordset RS;
            string SQL = "Select * from tblControlBoxes where cbID =" + ID.ToString();
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            if (!RS.EOF) RS.Delete();
            RS.Close();
        }

        public clsControlBox Item(byte ID)
        {
            int IDX = ListID(ID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cControlBoxes[IDX];
        }

        public void Load()
        {
            cControlBoxes.Clear();

            DAO.Recordset RS;
            string SQL = "Select * from tblControlBoxes order by cbNumber";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            while(!RS.EOF)
            {
                clsControlBox Box = new clsControlBox(mf);
                Box.Load((byte)RS.Fields["cbID"].Value);
                cControlBoxes.Add(Box);
                RS.MoveNext();
            }
            RS.Close();
        }
    }
}
