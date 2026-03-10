using System;
using System.IO;
using System.Windows.Forms;

namespace BinWatch
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
                AppConfig.Load();
                CopyDatabaseIfRequested();
                AppServices.Initialize();
                AppServices.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup error:\n\n{ex.Message}\n\n{ex.InnerException?.Message}",
                    "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(form);
        }

        private static void CopyDatabaseIfRequested()
        {
            if (!AppConfig.CopyDbOnStart) return;

            string src = AppConfig.CopyDbSource;
            string dst = AppConfig.ResolvedDbPath;

            // Clear the flag regardless of outcome so we don't retry on every start
            AppConfig.Save(AppConfig.DbPath, AppConfig.PassiveMode,
                copyDbOnStart: false, copyDbSource: "");
            AppConfig.CopyDbOnStart = false;
            AppConfig.CopyDbSource  = "";

            if (string.IsNullOrWhiteSpace(src) || !File.Exists(src)) return;
            if (File.Exists(dst)) return;  // destination already has a database

            string dstDir = Path.GetDirectoryName(dst);
            if (!Directory.Exists(dstDir))
            {
                MessageBox.Show(
                    $"Could not copy database — destination folder does not exist:\n{dstDir}",
                    "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            File.Copy(src, dst);
        }
    }
}
