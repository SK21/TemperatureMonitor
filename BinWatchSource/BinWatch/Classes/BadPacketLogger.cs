using System;
using System.IO;
using System.Net;

namespace BinWatch
{
    /// <summary>
    /// Logs rejected UDP packets to BinWatch_bad.log alongside the exe.
    /// Each entry includes the timestamp, source address, rejection reason, and a hex dump.
    /// Rolls over to BinWatch_bad.log.bak when the file exceeds 1 MB.
    /// </summary>
    public static class BadPacketLogger
    {
        private static readonly string LogPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch_bad.log");
        private static readonly string BackupPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch_bad.log.bak");
        private const long MaxBytes = 1024 * 1024; // 1 MB

        private static readonly object _lock = new object();

        /// <summary>
        /// Log a rejected packet with the reason and a hex dump of its bytes.
        /// </summary>
        public static void Log(string reason, IPEndPoint source, byte[] data)
        {
            try
            {
                string src = source != null ? source.ToString() : "unknown";
                string hex = data != null ? BitConverter.ToString(data).Replace("-", " ") : "(null)";
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{reason}] from {src}  {hex}";

                lock (_lock)
                {
                    RollIfNeeded();
                    using (var sw = File.AppendText(LogPath))
                        sw.WriteLine(line);
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
