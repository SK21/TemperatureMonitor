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

            // Catch unhandled exceptions on the UI thread
            Application.ThreadException += (s, e) =>
            {
                Logger.Error("Unhandled UI thread exception", e.Exception);
                MessageBox.Show(
                    $"An unexpected error occurred:\n\n{e.Exception.Message}\n\nDetails have been logged to BinWatch.log.",
                    "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Catch unhandled exceptions on background threads
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                Logger.Error("Unhandled background thread exception" + (e.IsTerminating ? " (fatal)" : ""), ex);
            };

            Logger.Info("BinWatch starting");

            try
            {
                AppConfig.Load();
                CopyDatabaseIfRequested();
                AppServices.Initialize();
                AppServices.Start();
            }
            catch (Exception ex)
            {
                Logger.Error("Startup failed", ex);

                if (!AppConfig.PassiveMode)
                {
                    var result = MessageBox.Show(
                        $"Startup failed:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\n" +
                        "Another instance may already have the database open.\n\n" +
                        "Start in passive mode (read-only view)?",
                        "BinWatch", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                    {
                        AppConfig.PassiveMode = true;
                        try
                        {
                            AppServices.Initialize();
                            AppServices.Start();
                        }
                        catch (Exception ex2)
                        {
                            Logger.Error("Startup failed in passive mode", ex2);
                            MessageBox.Show(
                                $"Startup error:\n\n{ex2.Message}\n\n{ex2.InnerException?.Message}\n\nDetails have been logged to BinWatch.log.",
                                "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"Startup error:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\nDetails have been logged to BinWatch.log.",
                        "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            MainForm form;
            try
            {
                form = new MainForm();
            }
            catch (Exception ex)
            {
                Logger.Error("Form load failed", ex);
                MessageBox.Show(
                    $"Form load error:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\nDetails have been logged to BinWatch.log.",
                    "BinWatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Info("BinWatch started");
            Application.Run(form);
            Logger.Info("BinWatch stopped");
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
