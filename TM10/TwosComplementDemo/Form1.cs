using System;
using System.Windows.Forms;

namespace TwosComplement
{
    public partial class Form1 : Form
    {
        private byte lsb;
        private byte msb;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            msb = byte.Parse(textBox1.Text);
            lsb = byte.Parse(textBox2.Text);
            FromTwos();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ToTwos();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //UInt16 s = Convert.ToUInt16(textBox4.Text, 16);
            UInt16 s = (ushort)uint.Parse(textBox4.Text, System.Globalization.NumberStyles.HexNumber);
            lsb = (byte)s;
            msb = (byte)(s >> 8);
            textBox1.Text = msb.ToString();
            textBox2.Text = lsb.ToString();
            FromTwos();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "255";
            textBox2.Text = "248";
        }

        private void FromTwos()
        {
            float r;
            UInt16 t = 0;
            UInt16 s = (ushort)((msb << 8) | lsb);
            if ((s & 32768) == 32768)   // 0b1000 0000 0000 0000
            {
                // negative number
                t = (ushort)(~s + 1);   // complement + 1
                r = (float)((t * -1.0));// multiply by -1 to give two's complement
                r = (float)(r / 16.0);  // convert one-wire temperatures
            }
            else
            {
                // positive number
                r = (float)(s / 16.0);
            }
            label1.Text = r.ToString();
        }

        private void ToTwos()
        {
            float r = (float)(float.Parse(textBox3.Text) * 16.0);
            if (r < 0)
            {
                r = (float)(r * -1.0);
                UInt16 t = (ushort)r;
                t = (ushort)(~t + 1);
                lsb = (byte)t;
                msb = (byte)(t >> 8);
            }
            else
            {
                lsb = (byte)r;
                for (int i = 0; i < 8; i++)
                {
                    r = (float)(r / 2.0);
                }

                msb = (byte)r;
            }
            label10.Text = lsb.ToString();
            label9.Text = msb.ToString();

            UInt16 h = (ushort)((msb << 8) | lsb);
            label12.Text = h.ToString("X");
        }

        private void label14_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(label14.Text);
        }
    }
}