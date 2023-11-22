using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using TempMonitor.Forms;

namespace TempMonitor
{
    public class clsControlBox
    {
        // tblControlBoxes
        // cbID - record ID
        // cbNumber - Controlbox ID
        // cbDescription
        // cbMac

        private int cBoxID;
        private string cDescription;
        private int cID;
        private bool cIsNew;
        private string cMac;
        private frmMain mf;

        public clsControlBox(frmMain CallingForm)
        {
            mf = CallingForm;
            cBoxID = 0;
            cDescription = "";
            cIsNew = true;
        }

        public int BoxID
        {
            get { return cBoxID; }
            set
            {
                if (cIsNew & !IsUniqueNumber(value)) throw new ArgumentException("Duplicate ID: " + value.ToString());
                cBoxID = value;
            }
        }

        public string Description
        {
            get { return cDescription; }
            set
            {
                if (value.Length > 20)
                {
                    value = value.Substring(0, 20);
                }
                cDescription = value;
            }
        }

        public int ID
        { get { return cID; } }

        public bool IsNew
        { get { return cIsNew; } }

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

        public bool IsUniqueNumber(int NewID)
        {
            bool Result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.connString))
                {
                    string SQL = "select * from tblControlBoxes where cbNumber = @ID";
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", NewID);
                        con.Open();
                        object res = cmd.ExecuteScalar();
                        Result = (Convert.ToInt32(res) < 1);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clControlBox/IsUniqueNumber: " + ex.Message);
            }
            return Result;
        }

        public bool IsValid()
        {
            bool Result = true;

            return Result;
        }

        public bool Load(int ID = 0, int ControlBoxNumber = 0)
        {
            bool Result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.connString))
                {
                    string SQL;
                    if (ID == 0)
                    {
                        SQL = "select * from tblControlBoxes where cbNumber = @Number";
                    }
                    else
                    {
                        SQL = "select * from tblControlBoxes where cbID = @ID";
                    }

                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        cmd.Parameters.AddWithValue("@Number", ControlBoxNumber);
                        cmd.Parameters.AddWithValue("@ID", ID);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cID = (int)reader[0];
                            cBoxID = (int)reader[1];
                            cDescription = (string)reader[2];
                            cMac = (string)reader[3];
                            cIsNew = false;
                            Result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clControlBox/Load: " + ex.Message);
            }
            return Result;
        }

        public bool Save()
        {
            bool Result = false;
            try
            {
                if (IsValid())
                {
                    using (SqlConnection con = new SqlConnection(Properties.Settings.Default.connString))
                    {
                        string SQL = "select * from tblControlBoxes where cbNumber = @ID";
                        SqlCommand cmd = new SqlCommand(SQL, con);
                        cmd.Parameters.AddWithValue("@ID", cID);
                        con.Open();
                        object res = cmd.ExecuteScalar();
                        con.Close();
                        if (Convert.ToInt32(res) > 0)
                        {
                            // update record
                            SQL = "Update tblControlBoxes Set cbNumber = @Number, cbDescription = @Description, cbMac = @Mac";
                            using (SqlCommand cmdUpdate = new SqlCommand(SQL, con))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Number", cBoxID);
                                cmdUpdate.Parameters.AddWithValue("@Description", cDescription);
                                cmdUpdate.Parameters.AddWithValue("@Mac", cMac);
                                con.Open();
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // new record
                            SQL = "INSERT INTO tblControlBoxes(cbNumber,cbDescription,cbMac) output INSERTED.cbID VALUES(@Number,@Description,@Mac)";
                            using (SqlCommand cmdNew = new SqlCommand(SQL, con))
                            {
                                cmdNew.Parameters.AddWithValue("@Number", cBoxID);
                                cmdNew.Parameters.AddWithValue("@Description", cDescription);
                                cmdNew.Parameters.AddWithValue("@Mac", cMac);
                                con.Open();
                                cmdNew.ExecuteNonQuery();

                                object tmp = cmdNew.ExecuteScalar();
                                if (tmp != null)
                                {
                                    int.TryParse(tmp.ToString(), out cID);
                                    Result = true;
                                    cIsNew = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clControlBox/Save: " + ex.Message);
            }
            return Result;
        }
    }
}