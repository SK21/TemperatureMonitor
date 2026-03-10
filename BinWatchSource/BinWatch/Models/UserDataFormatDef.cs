using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinWatch.Models
{
    /// <summary>
    /// Persisted format definition: describes how the two DS18B20 user-data bytes
    /// map to Bin / Cable / Sensor.  3 field rows (one per field), each specifying
    /// which byte (1 = low / UserData0, 2 = high / UserData1) and the inclusive bit
    /// range within that byte.
    /// </summary>
    [Table("UserDataFormats")]
    public class UserDataFormatDef
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        // Row 0 – 2: Description (Bin/Cable/Sensor/null), Byte (1 or 2), BitLow, BitHigh
        public string R0Desc { get; set; }  public int R0Byte { get; set; }  public int R0Low { get; set; }  public int R0High { get; set; }
        public string R1Desc { get; set; }  public int R1Byte { get; set; }  public int R1Low { get; set; }  public int R1High { get; set; }
        public string R2Desc { get; set; }  public int R2Byte { get; set; }  public int R2Low { get; set; }  public int R2High { get; set; }

        public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();

        public override string ToString() => Name ?? $"Format #{Id}";

        // ── Decode ────────────────────────────────────────────────────────────────

        /// <summary>Decode raw user data (ushort) into 0-based bin/cable/sensor.</summary>
        public (int bin, int cable, int sensor) Decode(ushort raw)
        {
            int bin = 0, cable = 0, sensor = 0;
            for (int i = 0; i < 3; i++)
            {
                var (desc, byteNum, low, high) = GetRow(i);
                if (string.IsNullOrEmpty(desc) || desc == "-") continue;
                int val = FieldValue(raw, byteNum, low, high);
                if (desc == "Bin")    bin    = val;
                if (desc == "Cable")  cable  = val;
                if (desc == "Sensor") sensor = val;
            }
            return (bin, cable, sensor);
        }

        public (string desc, int byteNum, int low, int high) GetRow(int i)
        {
            switch (i)
            {
                case 0:  return (R0Desc, R0Byte, R0Low, R0High);
                case 1:  return (R1Desc, R1Byte, R1Low, R1High);
                default: return (R2Desc, R2Byte, R2Low, R2High);
            }
        }

        public void SetRow(int i, string desc, int byteNum, int low, int high)
        {
            switch (i)
            {
                case 0: R0Desc = desc; R0Byte = byteNum; R0Low = low; R0High = high; break;
                case 1: R1Desc = desc; R1Byte = byteNum; R1Low = low; R1High = high; break;
                case 2: R2Desc = desc; R2Byte = byteNum; R2Low = low; R2High = high; break;
            }
        }

        /// <summary>Extract a bit-range field from raw user data.</summary>
        public static int FieldValue(ushort raw, int byteNum, int low, int high)
        {
            if (high < low || low < 0 || high > 7) return 0;
            int byteVal = (byteNum == 1) ? (raw & 0xFF) : ((raw >> 8) & 0xFF);
            int mask    = (1 << (high - low + 1)) - 1;
            return (byteVal >> low) & mask;
        }

        /// <summary>BinWatch native: Byte2=Bin(0-7), Byte1 bits4-7=Cable, Byte1 bits0-3=Sensor.</summary>
        public static UserDataFormatDef BinWatchDefault => new UserDataFormatDef
        {
            Name   = "BinWatch",
            R0Desc = "Bin",    R0Byte = 2, R0Low = 0, R0High = 7,
            R1Desc = "Cable",  R1Byte = 1, R1Low = 4, R1High = 7,
            R2Desc = "Sensor", R2Byte = 1, R2Low = 0, R2High = 3
        };
    }
}
