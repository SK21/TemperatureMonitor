using System;
using System.Windows.Forms;

namespace TempMonitor.Forms
{
    // useage:
    // frmInputBox IB = new frmInputBox("Password?");
    // IB.ShowDialog(this);
    // bool Result = IB.Cancelled();
    // string Tmp = IB.Answer();
    // IB.Close();

    public partial class frmInputBox : Form
    {
        private bool cCancelled = false;

        public frmInputBox(string Message, string Title = "")
        {
            InitializeComponent();
            label1.Text = Message;
            if (Title == "")
            {
                this.Text = "Temperature Monitor";
            }
            else
            {
                this.Text = Title;
            }
        }

        private void frmInputBox_Load(object sender, EventArgs e)
        {
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            cCancelled = true;
            textBox1.Text = "";
            this.Hide();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            cCancelled = false;
            this.Hide();
        }

        public string Answer()
        {
            return textBox1.Text;
        }

        public bool Cancelled()
        {
            return cCancelled;
        }
    }
}