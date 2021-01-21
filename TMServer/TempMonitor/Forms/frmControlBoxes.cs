using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class frmControlBoxes : Form
    {
        private clsControlBoxes Boxes;
        private int CurrentRow;
        private FormMain mf;
        private bool NewRecord;
        private bool Updating;

        public frmControlBoxes(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Boxes = new clsControlBoxes(mf);
            Boxes.Load();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewRecord = true;
            Updating = true;

            tbNumber.Text = "";
            tbDescription.Text = "";
            ckSleep.Checked = false;

            Updating = false;
            SetButtons(true);
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
        }

        private void btReload_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btRestart_Click(object sender, EventArgs e)
        {
            try
            {
                PGN25020 UpdateBox = new PGN25020(mf);
                UpdateBox.ControlBoxNumber = Convert.ToByte(tbNumber.Text);
                UpdateBox.UseSleep = ckSleep.Checked;
                UpdateBox.SleepInterval = mf.SleepInterval;
                UpdateBox.ControlBoxCount = mf.Dbase.ControlBoxCount();
                UpdateBox.SendDiagnostics = ckDiagnostics.Checked;
                UpdateBox.Restart = true;
                UpdateBox.Send();
                mf.Tls.TimedMessageBox("Restarting micro-controller.");
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmControlBoxes:btRestart_Click: " + ex.Message);
            }
        }

        private void btWrite_Click(object sender, EventArgs e)
        {
            try
            {
                PGN25020 UpdateBox = new PGN25020(mf);
                UpdateBox.ControlBoxNumber = Convert.ToByte(tbNumber.Text);
                UpdateBox.UseSleep = ckSleep.Checked;
                UpdateBox.SleepInterval = mf.SleepInterval;
                UpdateBox.ControlBoxCount = mf.Dbase.ControlBoxCount();
                UpdateBox.SendDiagnostics = ckDiagnostics.Checked;
                UpdateBox.Send();
                mf.Tls.TimedMessageBox("Writing data to controlbox.");
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmControlBoxes:btWrite_Click: " + ex.Message);
            }
        }

        private void butCancelEdit_Click(object sender, EventArgs e)
        {
            SetButtons(false);
            UpdateEditBoxes();
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Confirm Delete Controlbox?", "Delete Record", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    byte ID = Convert.ToByte(DGV.Rows[CurrentRow].Cells[0].Value);
                    Boxes.Delete(ID);
                }
                catch (Exception ex)
                {
                    mf.Tls.TimedMessageBox(ex.Message);
                }
                LoadData();
            }
        }

        private void butSaveEdit_Click(object sender, EventArgs e)
        {
            if (butSaveEdit.Text == "Close")
            {
                this.Close();
            }
            else
            {
                try
                {
                    clsControlBox Box;
                    if (DGV.Rows.Count == 0 | NewRecord)
                    {
                        // first record
                        Box = Boxes.Add();
                    }
                    else
                    {
                        Box = Boxes.Item(Convert.ToByte(DGV.Rows[CurrentRow].Cells[0].Value));
                    }

                    byte Result = 0;
                    byte.TryParse(tbNumber.Text, out Result);
                    Box.BoxID = Result;

                    Box.Description = tbDescription.Text;
                    Box.UseSleep = ckSleep.Checked;
                    Box.UseDiagnostics = ckDiagnostics.Checked;

                    Box.Save();
                    LoadData();

                    CurrentRow = mf.Tls.FindRecord(DGV, 1, Box.BoxID);
                    DGV.CurrentCell = DGV[1, CurrentRow];
                }
                catch (Exception ex)
                {
                    mf.Tls.TimedMessageBox(ex.Message);
                    mf.Tls.WriteErrorLog("frmControlBoxes:butSaveEdit_Click " + ex.Message);
                }
            }
            UpdateEditBoxes();
            SetButtons(false);
            NewRecord = false;
        }

        private void ckDiagnostics_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckSleep_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void DGV_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV.CurrentRow.Index != CurrentRow & !Updating)
            {
                CurrentRow = DGV.CurrentRow.Index;
                UpdateEditBoxes();
            }
        }

        private void frmControlBoxes_Load(object sender, EventArgs e)
        {
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            DGV.EnableHeadersVisualStyles = false;

            LoadData();
            DGV.CurrentCell = DGV.FirstDisplayedCell;

            mf.ReceiveDiagnostics.NewMessage += ReceiveDiagnostics_NewMessage;
        }

        private void LoadData()
        {
            int LastRow = CurrentRow;
            Updating = true;
            try
            {
                dataSet1.Clear();

                foreach (clsControlBox Box in Boxes.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw["ID"] = Box.ID;
                    Rw["Number"] = Box.BoxID;
                    Rw["Description"] = Box.Description;
                    Rw["Sleep"] = Box.UseSleep;
                    Rw["Diagnostics"] = Box.UseDiagnostics;
                    Rw["IP"] = Box.IPaddress;
                    Rw["Mac"] = Box.Mac;

                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmControlBoxes:LoadData " + ex.Message);
            }

            // attempt to return to last record
            try
            {
                DGV.CurrentCell = DGV.Rows[LastRow].Cells["Number"];
            }
            catch (Exception)
            {
            }
            Updating = false;

            UpdateEditBoxes();
        }

        private void ReceiveDiagnostics_NewMessage(object sender, string e)
        {
            if (!butCancelEdit.Enabled) LoadData();  // don't load data if editing
        }

        private void SetButtons(bool Edited)
        {
            if (!Updating)
            {
                if (Edited)
                {
                    butCancelEdit.Enabled = true;
                    butSaveEdit.Text = "Save";
                    DGV.Enabled = false;
                    btPrint.Enabled = false;
                    butDelete.Enabled = false;
                    btnNew.Enabled = false;
                    btReload.Enabled = false;
                    btRestart.Enabled = false;
                    btWrite.Enabled = false;
                }
                else
                {
                    butCancelEdit.Enabled = false;
                    butSaveEdit.Text = "Close";
                    DGV.Enabled = true;
                    btPrint.Enabled = true;
                    butDelete.Enabled = true;
                    btnNew.Enabled = true;
                    btReload.Enabled = true;
                    btRestart.Enabled = true;
                    btWrite.Enabled = true;
                }
            }
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbNumber_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void UpdateEditBoxes()
        {
            if (DGV.CurrentRow != null)
            {
                Updating = true;

                tbNumber.Text = mf.Tls.NV(DGV.CurrentRow, 1, true);
                tbDescription.Text = mf.Tls.NV(DGV.CurrentRow, 2);
                ckSleep.Checked = mf.Tls.StringToBool(mf.Tls.NV(DGV.CurrentRow, 3));
                ckDiagnostics.Checked = mf.Tls.StringToBool(mf.Tls.NV(DGV.CurrentRow, 4));

                Updating = false;
            }
        }
    }
}