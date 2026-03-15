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
        /// When true, Logger.Debug() writes to the log and bad packets are logged.
        /// Takes effect immediately without restart.
        /// </summary>
        public static bool DebugLogging { get; set; } = false;

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

        // ── Grid sort orders ──────────────────────────────────────────────────────

        public static string ModulesSortColumn    { get; set; } = "";
        public static bool   ModulesSortAscending { get; set; } = true;
        public static string TempsSortColumn      { get; set; } = "";
        public static bool   TempsSortAscending   { get; set; } = true;

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
                else if (key == "DebugLogging")    DebugLogging  = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                else if (key == "MainForm.Left")   { if (int.TryParse(val, out int v)) MainFormLeft   = v; }
                else if (key == "MainForm.Top")    { if (int.TryParse(val, out int v)) MainFormTop    = v; }
                else if (key == "MainForm.Width")  { if (int.TryParse(val, out int v)) MainFormWidth  = v; }
                else if (key == "MainForm.Height") { if (int.TryParse(val, out int v)) MainFormHeight = v; }
                else if (key == "Modules.SortColumn")    ModulesSortColumn    = val;
                else if (key == "Modules.SortAscending") { if (bool.TryParse(val, out bool v)) ModulesSortAscending = v; }
                else if (key == "Temps.SortColumn")      TempsSortColumn      = val;
                else if (key == "Temps.SortAscending")   { if (bool.TryParse(val, out bool v)) TempsSortAscending   = v; }
            }

            // If the configured DB folder doesn't exist (e.g. copied install from another PC),
            // fall back to the local default so the app still starts.
            if (!string.IsNullOrWhiteSpace(DbPath))
            {
                string dir = Path.GetDirectoryName(DbPath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                {
                    Logger.Warning($"DbPath folder not found ({dir}), falling back to local database");
                    DbPath = "";
                }
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
            DbPath        = dbPath;
            PassiveMode   = passiveMode;
            CopyDbOnStart = copyDbOnStart;
            CopyDbSource  = copyDbSource;
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
                $"DebugLogging={DebugLogging.ToString().ToLower()}",
                $"MainForm.Left={left}",
                $"MainForm.Top={top}",
                $"MainForm.Width={width}",
                $"MainForm.Height={height}",
                $"Modules.SortColumn={ModulesSortColumn}",
                $"Modules.SortAscending={ModulesSortAscending.ToString().ToLower()}",
                $"Temps.SortColumn={TempsSortColumn}",
                $"Temps.SortAscending={TempsSortAscending.ToString().ToLower()}"
            });
        }
    }
}
