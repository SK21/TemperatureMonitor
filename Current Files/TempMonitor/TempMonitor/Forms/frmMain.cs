using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempMonitor.Forms
{
    public partial class frmMain : Form
    {
        public clsTools Tls;
        public clsControlBox CB;
        public clsControlBoxes Boxes;
        private bool cDatabaseFound;

        public frmMain()
        {
            InitializeComponent();
            Tls = new clsTools(this);
            Boxes = new clsControlBoxes(this);

            CB = new clsControlBox(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CB.BoxID = 98;
            CB.Mac = "155";
            CB.Description = "Bin 8";
            CB.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CB.Load(0, 57);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CB.Load(0, 57);
            //CB.Delete();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Tls.PrevInstance())
            {
                Tls.ShowHelp("Already Running!");
                this.Close();
            }
            Tls.LoadFormData(this);

            // check database connection
            string LastCon = Tls.GetProperty("LastDatabase");
            cDatabaseFound = Tls.DatabaseFound(LastCon);
            if (!cDatabaseFound) Tls.ShowHelp("Database could not be loaded.", "Help", 20000, true);
            LoadSettings();
        }

        private void LoadSettings()
        {
            this.BackColor = Properties.Settings.Default.BackColour;
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Color.Black;
                c.BackColor = Properties.Settings.Default.BackColour;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) Tls.SaveFormData(this);
        }
    }
}