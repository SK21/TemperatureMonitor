using System;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class FormTimedMessage : Form
    {
        public FormTimedMessage(string str1, string str2 = "", int timeInMsec = 3000)
        {
            InitializeComponent();

            try
            {
                timer1.Interval = timeInMsec;

                lblMessage.Text = str1;
                lblMessage2.Text = str2;

                int MessWidth = lblMessage.Width;
                if (lblMessage2.Width > MessWidth) MessWidth = lblMessage2.Width;

                int MaxWidth = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
                int NewWidth = MessWidth + 30;

                if (NewWidth > MaxWidth)
                {
                    NewWidth = MaxWidth;
                    Width = NewWidth;
                }
                else
                {
                    Width = NewWidth;
                    int left = (NewWidth - MessWidth-8) / 2;
                    lblMessage.Left = left;
                    lblMessage2.Left = left;
                }

                if(str2=="")
                {
                    Height = 95;
                }
                else
                {
                    Height = 120;
                }
            }
            catch (Exception)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
            Close();
        }
    }
}