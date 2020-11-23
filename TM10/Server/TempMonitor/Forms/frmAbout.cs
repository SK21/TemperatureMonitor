using System;
using System.Reflection;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class frmAbout : Form
    {
        private FormMain mf;

        public frmAbout(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            lbVersion.Text = String.Format("Version:  {0}", AssemblyVersion);
            lbDate.Text = ((FormMain)this.Owner).Tls.VersionDate;
            lbDBver.Text = "Database Version: " + ((FormMain)this.Owner).Dbase.DBversion;
            lbName.Text = mf.Dbase.DBname(true);
            lbSize.Text = mf.Dbase.DBsize();
            lbFileDate.Text = mf.Dbase.DBdate();
            lbFolder.Text = mf.Dbase.DBfolder();
        }

        private void lbIPaddress_Click(object sender, EventArgs e)
        {

        }
    }
}