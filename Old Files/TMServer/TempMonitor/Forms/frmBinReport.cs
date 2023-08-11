using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class frmBinReport : Form
    {
        FormMain mf;

        public frmBinReport(FormMain CallingForm)
        {
            mf = CallingForm;
            InitializeComponent();
        }

        private void frmBinReport_Load(object sender, EventArgs e)
        {
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV.DefaultCellStyle.Format = "##.#";
            DGV.EnableHeadersVisualStyles = false;

            foreach(DataGridViewColumn Col in DGV.Columns)
            {
                Col.Width = 80;
                Col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            btMonth.PerformClick();
            btUpdate.PerformClick();
        }

        private void rbDay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbHour_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbSingle_CheckedChanged(object sender, EventArgs e)
        {
            dtpRange.Enabled = !rbSingle.Checked;

        }

        private void btToday_Click(object sender, EventArgs e)
        {
            rbSingle.Checked = true;
            dtpSingle.Value = DateTime.Now;
        }

        private void btWeek_Click(object sender, EventArgs e)
        {
            rbRange.Checked = true;
            dtpSingle.Value = DateTime.Now.AddDays(-6);
            dtpRange.Value = DateTime.Now;
        }

        private void btMonth_Click(object sender, EventArgs e)
        {
            rbRange.Checked = true;
            dtpSingle.Value = DateTime.Now.AddDays(-29);
            dtpRange.Value = DateTime.Now;
        }

        private void LoadTable()
        {
            try
            {
                DateTime StDate;
                DateTime EndDate;
                string DateFmt;
                DAO.Recordset RS;
                int ID = 0;

                // make tmp table
                string SQL1 = "SELECT recTimeStamp, senSensorNumber, recTemp INTO tmpRecs";
                SQL1 += " FROM tblSensors LEFT JOIN tblRecords ON tblSensors.senID = tblRecords.recSenID";
                SQL1 += " GROUP BY recTimeStamp,senSensorNumber,recTemp,senBinNumber,senCableNumber";
                SQL1 += " Having senBinNumber = " + tbBin.Text;
                SQL1 += " And senCableNumber = " + tbCable.Text;

                // date
                StDate = dtpSingle.Value;
                EndDate = dtpRange.Value;
                if (StDate == EndDate | rbSingle.Checked)
                {
                    // single date, end date is beginning of the next day
                    EndDate = StDate.AddDays(1);
                }
                else
                {
                    // date range, end date is beginning of the next day after user selected end date
                    EndDate = EndDate.AddDays(1);
                }
                SQL1 += " And recTimeStamp >= " + mf.Dbase.ToAccessDate(StDate) + " And recTimeStamp < " + mf.Dbase.ToAccessDate(EndDate);
                SQL1 += " Order By senBinNumber,recTimeStamp,senCableNumber,senSensorNumber";

                mf.Dbase.DeleteTable("tmpRecs");  // delete old table, if any

                mf.Dbase.DB.Execute(SQL1);   // create tmp table

                // make crosstab recordset
                if (rbHour.Checked)
                {
                    DateFmt = "" + (char)39 + "yyyy/mm/dd  hh" + (char)39;
                }
                else
                {
                    DateFmt = "" + (char)39 + "yyyy/mm/dd" + (char)39;
                }
                string SQL = "Transform Avg(recTemp) as AvgOfrecTemp";
                SQL += " Select Format([recTimeStamp]," + DateFmt + ") as ReadDate";
                SQL += " From tmpRecs";
                SQL += " Group By Format([recTimeStamp]," + DateFmt + ")";
                SQL += " PIVOT senSensorNumber In (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)";
                RS = mf.Dbase.DB.OpenRecordset(SQL);

                dataSet1.Clear();

                try  // needed to close recordset after error
                {
                    while (!RS.EOF)
                    {
                        ID++;
                        DataRow Rw = dataSet1.Tables[0].NewRow();
                        Rw[0] = ID;
                        Rw[1] = mf.Dbase.FieldToString(RS, "ReadDate");
                        for (int i = 0; i < 16; i++)
                        {
                            Rw[i + 2] = mf.Dbase.FieldToFloat(RS, (i + 1).ToString("N0"));
                        }

                        dataSet1.Tables[0].Rows.Add(Rw);
                        RS.MoveNext();
                    }

                }
                catch (Exception)
                {

                }
                RS.Close();
            }
            catch (Exception ex)
            {
                mf.Tls.TimedMessageBox(ex.Message);
                mf.Tls.WriteErrorLog("frmBinReport:LoadTable " + ex.Message);
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            btUpdate.Text = "Loading...";
            btUpdate.Enabled = false;
            LoadTable();
            btUpdate.Enabled = true;
            btUpdate.Text = "Update";
        }

        private void tbBin_Enter(object sender, EventArgs e)
        {
            tbBin.SelectionStart = 0;
            tbBin.SelectionLength = tbBin.Text.Length;
        }

        private void tbCable_Enter(object sender, EventArgs e)
        {
            tbCable.SelectionStart = 0;
            tbCable.SelectionLength = tbCable.Text.Length;

        }

        private void tbSensor_Enter(object sender, EventArgs e)
        {
            tbSensor.SelectionStart = 0;
            tbSensor.SelectionLength = tbSensor.Text.Length;
        }
    }
}
