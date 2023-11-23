using System;
using System.Drawing;
using System.Windows.Forms;

namespace TempMonitor.Forms
{
    public partial class frmMain : Form
    {
        public clsControlBoxes Boxes;
        public clsTools Tls;

        public frmMain()
        {
            InitializeComponent();
            Tls = new clsTools(this);
            Boxes = new clsControlBoxes(this);
        }
        public void WriteEvent(string Message)
        {
            tbEvents.Text += DateTime.Now.ToString() + " - " + Message + "\r\n";
            tbEvents.SelectionStart = tbEvents.Text.Length;
            tbEvents.ScrollToCaret();
            Tls.WriteActivityLog(Message);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) Tls.SaveFormData(this);
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
            Tls.CheckDatabase("", true, true);
            if (!Properties.Settings.Default.DBfound) Tls.ShowHelp("Database could not be loaded.", "Help", 20000, true);
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
            this.Text = "Temperature Monitor [" + Tls.FileName() + "]";
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string DBname = "";
            bool Found = false;
            openFileDialog1.InitialDirectory = Tls.DataFolder;
            do
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel) break;
                DBname = openFileDialog1.FileName;
                Found = Tls.CheckDatabase(DBname, true);
            } while (!Found);
            openFileDialog1.Dispose();
            if (Found) LoadSettings();
        }
    }
}