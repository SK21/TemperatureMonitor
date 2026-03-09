using System;
using System.Windows.Forms;

namespace BinTempsApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                AppServices.Initialize();
                AppServices.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup error:\n\n{ex.Message}\n\n{ex.InnerException?.Message}",
                    "BinTemps", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MainForm form;
            try
            {
                form = new MainForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form load error:\n\n{ex.Message}\n\n{ex.InnerException?.Message}",
                    "BinTemps", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(form);
        }
    }
}
