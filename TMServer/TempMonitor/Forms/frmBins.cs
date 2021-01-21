using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class frmBins : Form
    {
        private clsStorages Bins;
        private int CurrentRow;
        private FormMain mf;
        private bool NewRecord;
        private bool Updating;

        public frmBins(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Bins = new clsStorages(mf);
            Bins.Load();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewRecord = true;
            Updating = true;

            tbNumber.Text = "";
            tbDescription.Text = "";

            Updating = false;
            SetButtons(true);
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
        }

        private void butCancelEdit_Click(object sender, EventArgs e)
        {
            SetButtons(false);
            UpdateDisplay();
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Confirm Delete Bin?", "Delete Record", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    short ID = Convert.ToInt16(mf.Tls.NV(DGV.CurrentRow, 0, true));
                    Bins.Delete(ID);
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
                    clsStorage Bin;
                    if (DGV.Rows.Count == 0 | NewRecord)
                    {
                        Bin = Bins.Add();
                    }
                    else
                    {
                        Bin = Bins.Item(Convert.ToInt16(mf.Tls.NV(DGV.CurrentRow, 0, true)));

                    }

                    Bin.Number = Convert.ToInt16(tbNumber.Text);
                    Bin.Description = tbDescription.Text;
                    Bin.Save();

                    LoadData();

                    CurrentRow = mf.Tls.FindRecord(DGV, 1, Bin.Number);
                    DGV.CurrentCell = DGV[1, CurrentRow];
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                    {
                        mf.Tls.TimedMessageBox(ex.Message);
                    }
                    else
                    {
                        mf.Tls.WriteErrorLog("frmSensors:butSaveEdit_Click " + ex.Message);
                        mf.Tls.TimedMessageBox(ex.Message);
                    }
                }
                UpdateDisplay();
                SetButtons(false);
                NewRecord = false;
            }
        }

        private void DGV_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV.CurrentRow.Index != CurrentRow & !Updating)
            {
                CurrentRow = DGV.CurrentRow.Index;
                UpdateDisplay();
            }
        }

        private void DGV_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (!Updating)
            {
                if (butSaveEdit.Text == "Save")
                {
                    butSaveEdit.PerformClick();
                }
            }
        }

        private void Edit_Enter(object sender, EventArgs e)
        {
        }

        private void frmBins_Load(object sender, EventArgs e)
        {
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            DGV.EnableHeadersVisualStyles = false;

            LoadData();
            UpdateDisplay();
            DGV.CurrentCell = DGV.FirstDisplayedCell;
        }

        private void LoadData()
        {
            Updating = true;
            try
            {
                dataSet1.Clear();

                foreach (clsStorage Bin in Bins.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw[0] = Bin.ID;
                    Rw[1] = Bin.Number;
                    Rw[2] = Bin.Description;

                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmBins:LoadData " + ex.Message);
            }
            Updating = false;
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
                }
                else
                {
                    butCancelEdit.Enabled = false;
                    butSaveEdit.Text = "Close";
                    DGV.Enabled = true;
                    btPrint.Enabled = true;
                    butDelete.Enabled = true;
                    btnNew.Enabled = true;
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

        private void UpdateDisplay()
        {
            if (DGV.CurrentRow != null)
            {
                Updating = true;

                tbNumber.Text = mf.Tls.NV(DGV.CurrentRow, 1, true);
                tbDescription.Text = mf.Tls.NV(DGV.CurrentRow, 2);

                Updating = false;
            }
        }
    }
}