using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TempMonitor.Forms;

namespace TempMonitor
{
    public class clsControlBoxes
    {
        public IList<clsControlBox> Items;
        private List<clsControlBox> cControlBoxes = new List<clsControlBox>();
        private frmMain mf;

        public clsControlBoxes(frmMain CF)
        {
            mf = CF;
            Items = cControlBoxes.AsReadOnly();
        }

        public clsControlBox Add(clsControlBox NewControlBox)
        {
            cControlBoxes.Add(new clsControlBox(mf));
            clsControlBox Box = cControlBoxes[cControlBoxes.Count - 1];
            Box.BoxID = NewControlBox.BoxID;
            Box.Description = NewControlBox.Description;
            Box.Mac = NewControlBox.Mac;
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

        private int ListID(int ID)
        {
            for (int i = 0; i < cControlBoxes.Count; i++)
            {
                if (cControlBoxes[i].ID == ID) return i;
            }
            return -1;
        }

        public bool Delete(int ID)
        {
            bool Result = false;
            try
            {
                int IDX = ListID(ID);
                if (IDX != -1) throw new IndexOutOfRangeException();
                cControlBoxes.RemoveAt(IDX);

                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.connString))
                {
                    string SQL = "Delete from tblControlBoxes where cbID = @ID";
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", IDX);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clControlBoxes/Delete: " + ex.Message);
            }
            return Result;
        }

        public clsControlBox Item(byte ID)
        {
            int IDX = ListID(ID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cControlBoxes[IDX];
        }

        public bool Load()
        {
            bool Result = false;
            try
            {
                cControlBoxes.Clear();
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.connString))
                {
                    string SQL = "Select * from tblControlBoxes order by cbNumber";
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            clsControlBox Box = new clsControlBox(mf);
                            Box.Load((int)reader[0]);
                            cControlBoxes.Add(Box);
                        }
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clControlBoxes/Load: " + ex.Message);
            }
            return Result;
        }
    }
}