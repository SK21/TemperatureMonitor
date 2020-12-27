using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace TempMonitor.Forms
{
    public partial class frmOptions : Form
    {
        private readonly FormMain mf;

        private bool DataChanged = false;

        public frmOptions(FormMain CallingForm)
        {
            mf = CallingForm;

            InitializeComponent();
        }
        private void butCancel_Click(object sender, EventArgs e)
        {
            LoadData();
            DataChanged = false;
            SetButtons();
            ckRecording.Select();
        }

        private void butDefaults_Click(object sender, EventArgs e)
        {
            tbSaveInterval.Text = "12";
            tbRecordInterval.Text = "15";
            tbDelay.Text = "30";
            ckAutoSave.Checked = true;
            ckRecording.Checked = true;
            tbSaveLocation.Text = mf.Tls.DataFolder;
            tbMaxBoxes.Text = "5";
            tbSleep.Text = "0";
            ckListen.Checked = false;
        }

        private void butLocation_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) tbSaveLocation.Text = folderBrowserDialog1.SelectedPath;
            folderBrowserDialog1.Dispose();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            if (butSave.Text == "Close")
            {
                this.Close();
            }

            else
            {
                // check save location
                try
                {
                    if (!Directory.Exists(tbSaveLocation.Text)) Directory.CreateDirectory(tbSaveLocation.Text);
                }
                catch (Exception)
                {
                    mf.Tls.WriteErrorLog("Could not create folder.");
                    tbSaveLocation.Text = tbSaveLocation.Tag.ToString();
                }

                SaveData();
                LoadData();
                DataChanged = false;
                SetButtons();
                ckRecording.Select();
            }
        }

        private void ckAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }
        private void ckRecording_CheckedChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            LoadData();
            DataChanged = false;
            SetButtons();
        }
        private void LoadData()
        {
            DAO.Recordset RS;
            string SQL = "Select * from tblProps";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            tbSaveInterval.Text = Convert.ToString(RS.Fields["dbSaveInterval"].Value);
            tbRecordInterval.Text = Convert.ToString(RS.Fields["dbRecordInterval"].Value);
            tbDelay.Text = Convert.ToString(RS.Fields["dbControlBoxDelay"].Value);
            ckAutoSave.Checked = Convert.ToBoolean(RS.Fields["dbSaveReport"].Value);
            ckRecording.Checked = Convert.ToBoolean(RS.Fields["dbRecordData"].Value);
            ckListen.Checked = Convert.ToBoolean(RS.Fields["dbListenOnly"].Value);
            tbSaveLocation.Text = Convert.ToString(RS.Fields["dbSaveLocation"].Value);
            tbMaxBoxes.Text = Convert.ToString(RS.Fields["dbMaxBoxes"].Value);
            int Sleep = mf.Dbase.FieldToInt(RS, "dbSleepInterval");
            tbSleep.Text = Convert.ToString(Sleep / 60);
            RS.Close();

            // backup values
            tbSaveInterval.Tag = tbSaveInterval.Text;
            tbRecordInterval.Tag = tbRecordInterval.Text;
            tbDelay.Tag = tbDelay.Text;
            ckAutoSave.Tag = ckAutoSave.Checked;
            ckRecording.Tag = ckRecording.Checked;
            tbSaveLocation.Tag = tbSaveLocation.Text;
            tbMaxBoxes.Tag = tbMaxBoxes.Text;
            tbSleep.Tag = tbSleep.Text;
        }

        private void SaveData()
        {
            try
            {
                DAO.Recordset RS;
                string SQL = "Select * from tblProps";
                RS = mf.Dbase.DB.OpenRecordset(SQL);
                RS.Edit();
                RS.Fields["dbSaveInterval"].Value = mf.Tls.StringToInt(tbSaveInterval.Text);
                RS.Fields["dbRecordInterval"].Value = mf.Tls.StringToInt(tbRecordInterval.Text);
                RS.Fields["dbControlBoxDelay"].Value = mf.Tls.StringToInt(tbDelay.Text);
                RS.Fields["dbSaveReport"].Value = Convert.ToBoolean(ckAutoSave.Checked);
                RS.Fields["dbRecordData"].Value = Convert.ToBoolean(ckRecording.Checked);
                RS.Fields["dbListenOnly"].Value = Convert.ToBoolean(ckListen.Checked);
                RS.Fields["dbSaveLocation"].Value = Convert.ToString(tbSaveLocation.Text);
                RS.Fields["dbMaxBoxes"].Value = mf.Tls.StringToInt(tbMaxBoxes.Text);
                RS.Fields["dbSleepInterval"].Value = mf.Tls.StringToInt(tbSleep.Text)*60;
                RS.Update();
                RS.Close();

                // backup values
                tbSaveInterval.Tag = tbSaveInterval.Text;
                tbRecordInterval.Tag = tbRecordInterval.Text;
                tbDelay.Tag = tbDelay.Text;
                ckAutoSave.Tag = ckAutoSave.Checked;
                ckRecording.Tag = ckRecording.Checked;
                tbSaveLocation.Tag = tbSaveLocation.Text;
                tbMaxBoxes.Tag = tbMaxBoxes.Text;
                tbSleep.Tag = tbSleep.Text;
            }
            catch (Exception ex)
            {
                mf.Tls.TimedMessageBox("Could not save data", ex.Message);
                mf.Tls.WriteErrorLog("frmOptions: SaveData: " + ex.Message);
            }
        }

        private void SetButtons()
        {
            if (DataChanged)
            {
                butCancel.Enabled = true;
                butSave.Text = "Save";
            }
            else
            {
                butCancel.Enabled = false;
                butSave.Text = "Close";
            }
        }

        private void tbDelay_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void tbDelay_Validating(object sender, CancelEventArgs e)
        {
            int i = mf.Tls.StringToInt(tbDelay.Text);
            if (i < 1 | i > 600)
            {
                mf.Tls.TimedMessageBox("Invalid entry", "Must be between 1 and 600");
                e.Cancel = true;
                tbDelay.Text = tbDelay.Tag.ToString();
            }
            else
            {
                tbDelay.Text = i.ToString();
            }
        }

        private void tbRecordInterval_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }
        private void tbRecordInterval_Validating(object sender, CancelEventArgs e)
        {
            int i = mf.Tls.StringToInt(tbRecordInterval.Text);
            if (i < 1 | i > 10000)
            {
                mf.Tls.TimedMessageBox("Invalid entry", "Must be between 1 and 10,000");
                e.Cancel = true;
                tbRecordInterval.Text = tbRecordInterval.Tag.ToString();
            }
            else
            {
                tbRecordInterval.Text = i.ToString();
            }
        }

        private void tbSaveInterval_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void tbSaveInterval_Validating(object sender, CancelEventArgs e)
        {
            int i = mf.Tls.StringToInt(tbSaveInterval.Text);
            if (i < 0 | i > 200)
            {
                mf.Tls.TimedMessageBox("Invalid entry", "Must be between 0 and 200");
                e.Cancel = true;
                tbSaveInterval.Text = tbSaveInterval.Tag.ToString();
            }
            else
            {
                tbSaveInterval.Text = i.ToString();
            }
        }

        private void tbSaveLocation_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void tbMaxBoxes_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void tbMaxBoxes_Validating(object sender, CancelEventArgs e)
        {
            int i = mf.Tls.StringToInt(tbMaxBoxes.Text);
            if(i<1|i>255)
            {
                mf.Tls.TimedMessageBox("Invalid entry", "Must be between 1 and 255");
                e.Cancel = true;
                tbMaxBoxes.Text = tbMaxBoxes.Tag.ToString();
            }
            else
            {
                tbMaxBoxes.Text = i.ToString();
            }
        }

        private void ckListen_CheckedChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void tbSleep_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void tbSleep_Validating(object sender, CancelEventArgs e)
        {
            int i = mf.Tls.StringToInt(tbSleep.Text);
            if (i < 0 | i > 23)
            {
                mf.Tls.TimedMessageBox("Invalid entry", "Must be between 0 and 23");
                e.Cancel = true;
                tbSleep.Text = tbSleep.Tag.ToString();
            }
            else
            {
                tbSleep.Text = i.ToString();
            }
        }
    }
}