using System;
using System.IO;

namespace BinWatch
{
    /// <summary>
    /// Persists app-level settings to BinWatch.ini alongside the exe.
    /// Loaded once in Program.Main before AppServices.Initialize().
    /// </summary>
    public static class AppConfig
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch.ini");

        // ── Database ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Explicit path to the database file. Empty string means use the default
        /// (BinWatch.db in the same folder as the exe).
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
        /// startup, then clear this flag.
        /// </summary>
        public static bool CopyDbOnStart { get; set; } = false;

        /// <summary>The database file to copy from when CopyDbOnStart is true.</summary>
        public static string CopyDbSource { get; set; } = "";

        /// <summary>
        /// Resolved database path — DbPath if set, otherwise the default location.
        /// </summary>
        public static string ResolvedDbPath =>
            string.IsNullOrWhiteSpace(DbPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch.db")
                : DbPath;

        // ── Main form bounds ──────────────────────────────────────────────────────
        // -1 means not yet saved; form falls back to its default CenterScreen position.

        public static int MainFormLeft   { get; set; } = -1;
        public static int MainFormTop    { get; set; } = -1;
        public static int MainFormWidth  { get; set; } = -1;
        public static int MainFormHeight { get; set; } = -1;

        // ── Load / Save ───────────────────────────────────────────────────────────

        public static void Load()
        {
            if (!File.Exists(FilePath)) return;

            foreach (var line in File.ReadAllLines(FilePath))
            {
                int eq = line.IndexOf('=');
                if (eq < 0) continue;
                string key = line.Substring(0, eq).Trim();
                string val = line.Substring(eq + 1).Trim();

                if      (key == "DbPath")          DbPath        = val;
                else if (key == "PassiveMode")     PassiveMode   = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                else if (key == "CopyDbOnStart")   CopyDbOnStart = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                else if (key == "CopyDbSource")    CopyDbSource  = val;
                else if (key == "MainForm.Left")   { if (int.TryParse(val, out int v)) MainFormLeft   = v; }
                else if (key == "MainForm.Top")    { if (int.TryParse(val, out int v)) MainFormTop    = v; }
                else if (key == "MainForm.Width")  { if (int.TryParse(val, out int v)) MainFormWidth  = v; }
                else if (key == "MainForm.Height") { if (int.TryParse(val, out int v)) MainFormHeight = v; }
            }
        }

        /// <summary>
        /// Writes database/mode settings to BinWatch.ini and updates in-memory
        /// values so that a subsequent SaveFormBounds() call does not overwrite
        /// them with stale data. The new DB path takes effect on next start.
        /// </summary>
        /// <remarks>
        /// Use <see cref="SaveToFileOnly"/> when the new settings should not
        /// affect the currently running session (e.g. saving from the Settings
        /// dialog while the app is still active).
        /// </remarks>
        public static void Save(string dbPath, bool passiveMode,
            bool copyDbOnStart = false, string copyDbSource = "")
        {
            DbPath        = dbPath;
            PassiveMode   = passiveMode;
            CopyDbOnStart = copyDbOnStart;
            CopyDbSource  = copyDbSource;
            WriteIni(dbPath, passiveMode, copyDbOnStart, copyDbSource,
                MainFormLeft, MainFormTop, MainFormWidth, MainFormHeight);
        }

        /// <summary>
        /// Writes database/mode settings to BinWatch.ini WITHOUT updating the
        /// in-memory properties, so the currently running session is unaffected.
        /// Changes take effect on the next restart.
        /// </summary>
        public static void SaveToFileOnly(string dbPath, bool passiveMode,
            bool copyDbOnStart = false, string copyDbSource = "")
        {
            WriteIni(dbPath, passiveMode, copyDbOnStart, copyDbSource,
                MainFormLeft, MainFormTop, MainFormWidth, MainFormHeight);
        }

        /// <summary>
        /// Saves the main form position and size immediately (called on FormClosing).
        /// Updates in-memory bounds and writes the full ini.
        /// </summary>
        public static void SaveFormBounds(int left, int top, int width, int height)
        {
            MainFormLeft   = left;
            MainFormTop    = top;
            MainFormWidth  = width;
            MainFormHeight = height;
            WriteIni(DbPath, PassiveMode, CopyDbOnStart, CopyDbSource,
                left, top, width, height);
        }

        private static void WriteIni(string dbPath, bool passiveMode,
            bool copyDbOnStart, string copyDbSource,
            int left, int top, int width, int height)
        {
            File.WriteAllLines(FilePath, new[]
            {
                $"DbPath={dbPath}",
                $"PassiveMode={passiveMode.ToString().ToLower()}",
                $"CopyDbOnStart={copyDbOnStart.ToString().ToLower()}",
                $"CopyDbSource={copyDbSource}",
                $"MainForm.Left={left}",
                $"MainForm.Top={top}",
                $"MainForm.Width={width}",
                $"MainForm.Height={height}"
            });
        }
    }
}
