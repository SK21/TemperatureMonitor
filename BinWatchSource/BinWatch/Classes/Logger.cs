using System;
using System.IO;

namespace BinWatch
{
    /// <summary>
    /// Simple thread-safe file logger. Writes to BinWatch.log alongside the exe.
    /// Rolls over to BinWatch.log.bak when the file exceeds 1 MB.
    /// </summary>
    public static class Logger
    {
        private static readonly string LogPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch.log");
        private static readonly string BackupPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch.log.bak");
        private const long MaxBytes = 1024 * 1024; // 1 MB

        private static readonly object _lock = new object();

        public static void Info(string message) => Write("INFO ", message, null);
        public static void Warning(string message) => Write("WARN ", message, null);
        public static void Error(string message, Exception ex = null) => Write("ERROR", message, ex);

        private static void Write(string level, string message, Exception ex)
        {
            try
            {
                lock (_lock)
                {
                    RollIfNeeded();

                    using (var sw = File.AppendText(LogPath))
                    {
                        sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}");
                        if (ex != null)
                        {
                            sw.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
                            if (ex.InnerException != null)
                                sw.WriteLine($"  Inner: {ex.InnerException.Message}");
                            sw.WriteLine($"  {ex.StackTrace}");
                        }
                    }
                }
            }
            catch
            {
                // Logging must never crash the app
            }
        }

        private static void RollIfNeeded()
        {
            if (!File.Exists(LogPath)) return;
            if (new FileInfo(LogPath).Length < MaxBytes) return;

            try
            {
                if (File.Exists(BackupPath)) File.Delete(BackupPath);
                File.Move(LogPath, BackupPath);
            }
            catch { }
        }
    }
}
