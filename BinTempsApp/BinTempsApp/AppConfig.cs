using System;
using System.IO;

namespace BinTempsApp
{
    /// <summary>
    /// Persists app-level settings to BinTemps.ini alongside the exe.
    /// Loaded once in Program.Main before AppServices.Initialize().
    /// </summary>
    public static class AppConfig
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinTemps.ini");

        /// <summary>
        /// Explicit path to the database file. Empty string means use the default
        /// (BinTemps.db in the same folder as the exe).
        /// </summary>
        public static string DbPath { get; set; } = "";

        /// <summary>
        /// When true the app opens the database read-only and does not start the
        /// UDP server. Used on secondary machines that share a database over
        /// OneDrive, Sync.com, etc.
        /// </summary>
        public static bool PassiveMode { get; set; } = false;

        /// <summary>
        /// When true, Program.Main will copy CopyDbSource to ResolvedDbPath on
        /// startup, then clear this flag. Set by SettingsForm when the user
        /// requests a database copy to the new location.
        /// </summary>
        public static bool CopyDbOnStart { get; set; } = false;

        /// <summary>
        /// The database file to copy from when CopyDbOnStart is true.
        /// </summary>
        public static string CopyDbSource { get; set; } = "";

        /// <summary>
        /// Resolved database path — DbPath if set, otherwise the default location.
        /// </summary>
        public static string ResolvedDbPath =>
            string.IsNullOrWhiteSpace(DbPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinTemps.db")
                : DbPath;

        public static void Load()
        {
            if (!File.Exists(FilePath)) return;

            foreach (var line in File.ReadAllLines(FilePath))
            {
                int eq = line.IndexOf('=');
                if (eq < 0) continue;
                string key = line.Substring(0, eq).Trim();
                string val = line.Substring(eq + 1).Trim();

                if      (key == "DbPath")        DbPath        = val;
                else if (key == "PassiveMode")   PassiveMode   = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                else if (key == "CopyDbOnStart") CopyDbOnStart = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                else if (key == "CopyDbSource")  CopyDbSource  = val;
            }
        }

        /// <summary>
        /// Writes settings to BinTemps.ini without changing the in-memory values.
        /// The in-memory values only update on the next startup when Load() is called,
        /// so the running app is unaffected until restarted.
        /// </summary>
        public static void Save(string dbPath, bool passiveMode,
            bool copyDbOnStart = false, string copyDbSource = "")
        {
            File.WriteAllLines(FilePath, new[]
            {
                $"DbPath={dbPath}",
                $"PassiveMode={passiveMode.ToString().ToLower()}",
                $"CopyDbOnStart={copyDbOnStart.ToString().ToLower()}",
                $"CopyDbSource={copyDbSource}"
            });
        }
    }
}
