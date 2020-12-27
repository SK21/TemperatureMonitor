using System;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class FormTimedMessage : Form
    {
        public FormTimedMessage(string str1, string str2 = "", int timeInMsec = 3000)
        {
            InitializeComponent();

            lblMessage.Text = str1;
            lblMessage2.Text = str2;

            timer1.Interval = timeInMsec;

            int messWidth = str1.Length;
            if (str2.Length > messWidth) messWidth = str2.Length;
            int MaxWidth = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            int NewWidth = messWidth * 15 + 50;
            if (NewWidth > MaxWidth) NewWidth = MaxWidth;
            Width = NewWidth;
        }

        private void FormTimedMessage_Load(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
            Close();
        }
    }
}