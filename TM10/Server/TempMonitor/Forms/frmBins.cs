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
        private bool Updating;
        private bool NewRecord;

        public frmBins(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Bins = new clsStorages(mf);
            Bins.Load();
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
                    short ID = Convert.ToInt16(NV(0));
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
                    if (NewRecord)
                    {
                        Bin = Bins.Add();
                    }
                    else
                    {
                        Bin = Bins.Item(Convert.ToInt16(NV(0)));
                    }

                    Bin.Number = Convert.ToInt16(tbNumber.Text);
                    Bin.Description = tbDescription.Text;
                    Bin.Save();

                    LoadData();

                    CurrentRow = FindRecord(1, Bin.Number);
                    DGV.Rows[CurrentRow].Selected = true;
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

        private int FindRecord(int Cell, int Key)
        {
            try
            {
                foreach (DataGridViewRow RW in DGV.Rows)
                {
                    if (Convert.ToInt32(RW.Cells[Cell].Value) == Key)
                    {
                        return RW.Index;
                    }
                }
            }
            catch (Exception)
            {

            }
            return 0;
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

        private string NV(int Cell, bool IsNumber = true, DataGridViewRow RW = null)
        {
            if (RW == null) RW = DGV.Rows[CurrentRow];

            string Val = "";
            try
            {
                Val = RW.Cells[Cell].Value.ToString();
            }
            catch (Exception)
            {
            }
            if ((Val == "") & IsNumber) Val = "0";
            return Val;
        }

        private void SetButtons(bool Edited)
        {
            if (!Updating)
            {
                if (Edited)
                {
                    butCancelEdit.Enabled = true;
                    butSaveEdit.Text = "Save";
                }
                else
                {
                    butCancelEdit.Enabled = false;
                    butSaveEdit.Text = "Close";
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

                tbNumber.Text = NV(1, false);
                tbDescription.Text = NV(2, false);

                Updating = false;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewRecord = true;
            Updating = true;

            tbNumber.Text = "";
            tbDescription.Text = "";

            Updating = false;

        }
    }
}