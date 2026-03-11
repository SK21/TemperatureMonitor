using System.Data.Entity;
using System.Data.SQLite;
using BinWatch.Models;

namespace BinWatch.Data
{
    public class AppDbContext : DbContext
    {
        public static string DbPath => AppConfig.ResolvedDbPath;

        public AppDbContext()
            : base(new SQLiteConnection($"Data Source={DbPath};BusyTimeout=5000"), contextOwnsConnection: true)
        {
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<TemperatureRecord> Records { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<UserDataFormatDef> Formats { get; set; }

        /// <summary>
        /// Creates all tables if they do not already exist. Safe to call on every startup.
        /// Uses raw SQL because EF6's Database.Create() is unreliable with SQLite.
        /// </summary>
        public void EnsureSchema()
        {
            // WAL mode allows concurrent readers + one writer without "database is locked"
            // errors when the background UDP thread writes while the UI thread reads.
            // This setting is persistent — only needs to be applied once per database file.
            Database.ExecuteSqlCommand(
                System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction,
                "PRAGMA journal_mode=WAL;");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS Modules (
                    MacAddress      TEXT    PRIMARY KEY NOT NULL,
                    ModuleId        INTEGER NOT NULL DEFAULT 0,
                    Name            TEXT,
                    LastKnownIp     TEXT,
                    LastSeen        TEXT,
                    Status          TEXT,
                    FirmwareVersion INTEGER NOT NULL DEFAULT 0
                )");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS Sensors (
                    RomCode   TEXT    PRIMARY KEY NOT NULL,
                    ModuleMac TEXT,
                    BinId     INTEGER NOT NULL DEFAULT 0,
                    CableId   INTEGER NOT NULL DEFAULT 0,
                    SensorNum INTEGER NOT NULL DEFAULT 0,
                    Enabled   INTEGER NOT NULL DEFAULT 1,
                    Offset    REAL    NOT NULL DEFAULT 0.0,
                    MaxTemp   REAL    NOT NULL DEFAULT 40.0,
                    Label          TEXT,
                    RawUserData    INTEGER,
                    ManualLocation INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (ModuleMac) REFERENCES Modules (MacAddress)
                )");

            // Migration: add columns to existing databases that pre-date this schema version.
            try { Database.ExecuteSqlCommand("ALTER TABLE Sensors ADD COLUMN RawUserData INTEGER"); }
            catch { /* column already exists */ }
            try { Database.ExecuteSqlCommand("ALTER TABLE Sensors ADD COLUMN ManualLocation INTEGER NOT NULL DEFAULT 0"); }
            catch { /* column already exists */ }

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS UserDataFormats (
                    Id     INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name   TEXT,
                    R0Desc TEXT, R0Byte INTEGER NOT NULL DEFAULT 1, R0Low INTEGER NOT NULL DEFAULT 0, R0High INTEGER NOT NULL DEFAULT 7,
                    R1Desc TEXT, R1Byte INTEGER NOT NULL DEFAULT 1, R1Low INTEGER NOT NULL DEFAULT 0, R1High INTEGER NOT NULL DEFAULT 7,
                    R2Desc TEXT, R2Byte INTEGER NOT NULL DEFAULT 1, R2Low INTEGER NOT NULL DEFAULT 0, R2High INTEGER NOT NULL DEFAULT 7,
                    R3Desc TEXT, R3Byte INTEGER NOT NULL DEFAULT 1, R3Low INTEGER NOT NULL DEFAULT 0, R3High INTEGER NOT NULL DEFAULT 7
                )");

            // Migration: add FormatId to existing Sensors tables
            try { Database.ExecuteSqlCommand("ALTER TABLE Sensors ADD COLUMN FormatId INTEGER REFERENCES UserDataFormats(Id)"); }
            catch { /* column already exists */ }

            // Migration: add Offset columns to UserDataFormats
            try { Database.ExecuteSqlCommand("ALTER TABLE UserDataFormats ADD COLUMN R0Offset INTEGER NOT NULL DEFAULT 0"); }
            catch { /* column already exists */ }
            try { Database.ExecuteSqlCommand("ALTER TABLE UserDataFormats ADD COLUMN R1Offset INTEGER NOT NULL DEFAULT 0"); }
            catch { /* column already exists */ }
            try { Database.ExecuteSqlCommand("ALTER TABLE UserDataFormats ADD COLUMN R2Offset INTEGER NOT NULL DEFAULT 0"); }
            catch { /* column already exists */ }

            // Clear lock on sensors that have a format assigned — format decodes automatically, no lock needed
            Database.ExecuteSqlCommand("UPDATE Sensors SET ManualLocation = 0 WHERE FormatId IS NOT NULL");

            // Seed BinWatch format if not present
            Database.ExecuteSqlCommand(
                "INSERT OR IGNORE INTO UserDataFormats (Id, Name, R0Desc, R0Byte, R0Low, R0High, R1Desc, R1Byte, R1Low, R1High, R2Desc, R2Byte, R2Low, R2High) " +
                "VALUES (1, 'BinWatch', 'Bin', 2, 0, 7, 'Cable', 1, 4, 7, 'Sensor', 1, 0, 3)");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS TemperatureRecords (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    RomCode     TEXT,
                    Temperature REAL    NOT NULL DEFAULT 0.0,
                    Timestamp   TEXT    NOT NULL,
                    FOREIGN KEY (RomCode) REFERENCES Sensors (RomCode)
                )");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS Settings (
                    Key   TEXT PRIMARY KEY NOT NULL,
                    Value TEXT
                )");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sensor>()
                .HasOptional(s => s.Module)
                .WithMany(m => m.Sensors)
                .HasForeignKey(s => s.ModuleMac);

            modelBuilder.Entity<Sensor>()
                .HasOptional(s => s.Format)
                .WithMany(f => f.Sensors)
                .HasForeignKey(s => s.FormatId);

            modelBuilder.Entity<TemperatureRecord>()
                .HasOptional(r => r.Sensor)
                .WithMany(s => s.Records)
                .HasForeignKey(r => r.RomCode);
        }
    }
}
