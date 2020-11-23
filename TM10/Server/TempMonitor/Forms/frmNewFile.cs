using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TempMonitor.Forms
{
    public partial class frmNewFile : Form
    {
        private bool DataChanged = false;
        private FormMain mf;

        public frmNewFile(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            tbNewFileName.Text = "";
            ckSensors.Checked = false;
            ckRecords.Checked = false;
            ckBins.Checked = false;
            DataChanged = false;
            SetButtons();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            if (butSave.Text == "Save")
            {
                string NewFile = tbNewFileName.Text;
                if (mf.Dbase.LegalFileName(NewFile))
                {
                    NewFile = mf.Tls.DataFolder + "\\" + NewFile;
                    if (mf.Dbase.NewDatabase(NewFile, ckBins.Checked, ckSensors.Checked, ckRecords.Checked, true))
                    {
                        mf.WriteEvent("New file created.");
                        this.Close();
                    }
                    else
                    {
                        mf.Tls.TimedMessageBox("Could not create new file.");
                    }
                }
                else
                {
                    mf.Tls.TimedMessageBox("Not legal file name.");
                }
            }
            else
            {
                this.Close();
            }
        }

        private void ckBins_CheckedChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void ckRecords_CheckedChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void ckSensors_CheckedChanged(object sender, EventArgs e)
        {
            DataChanged = true;
            SetButtons();
        }

        private void frmNewFile_Load(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void label2_Click(object sender, EventArgs e)
        {
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

        private void tbNewFileName_TextChanged(object sender, EventArgs e)
        {
            DataChanged = (tbNewFileName.Text != "");
            SetButtons();
        }

        private void tbNewFileName_Validating(object sender, CancelEventArgs e)
        {
            tbNewFileName.Text = tbNewFileName.Text.Split('.')[0];  // remove file name extension
        }
    }
}