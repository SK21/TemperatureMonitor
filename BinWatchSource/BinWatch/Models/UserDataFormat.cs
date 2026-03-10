using System.Collections.Generic;

namespace BinWatch.Models
{
    /// <summary>
    /// Describes how a commercial system encodes bin/cable/sensor location
    /// into the DS18B20's 16-bit user data field.
    ///
    /// Each field is extracted as:  value = (raw >> Shift) &amp; ((1 &lt;&lt; Bits) - 1)
    ///
    /// Built-in:
    ///   BinWatch  — [15:8] Bin (8 bits)  [7:4] Cable (4 bits)  [3:0] Sensor (4 bits)
    ///
    /// Commercial formats (Opticable, BinSense, etc.) vary — use the preset dropdown
    /// in the Convert Sensors form to enter the correct shift/bits for each field,
    /// then save as a custom preset.
    /// </summary>
    public class UserDataFormat
    {
        public string Name        { get; set; }
        public int    BinShift    { get; set; }
        public int    BinBits     { get; set; }
        public int    CableShift  { get; set; }
        public int    CableBits   { get; set; }
        public int    SensorShift { get; set; }
        public int    SensorBits  { get; set; }

        public int DecodeBin   (ushort raw) => BinBits    == 0 ? 0 : (raw >> BinShift)    & ((1 << BinBits)    - 1);
        public int DecodeCable (ushort raw) => CableBits  == 0 ? 0 : (raw >> CableShift)  & ((1 << CableBits)  - 1);
        public int DecodeSensor(ushort raw) => SensorBits == 0 ? 0 : (raw >> SensorShift) & ((1 << SensorBits) - 1);

        public override string ToString() => Name;

        // ── Built-in formats ──────────────────────────────────────────────────────

        /// <summary>BinWatch native: [15:8]=Bin  [7:4]=Cable  [3:0]=Sensor</summary>
        public static readonly UserDataFormat BinWatch = new UserDataFormat
        {
            Name        = "BinWatch",
            BinShift    = 8, BinBits    = 8,
            CableShift  = 4, CableBits  = 4,
            SensorShift = 0, SensorBits = 4
        };

        /// <summary>
        /// Placeholder for Opticable format.
        /// Update Shift/Bits values once the actual encoding is confirmed.
        /// </summary>
        public static readonly UserDataFormat Opticable = new UserDataFormat
        {
            Name        = "Opticable (verify layout)",
            BinShift    = 0, BinBits    = 0,    // fill in once confirmed
            CableShift  = 8, CableBits  = 8,
            SensorShift = 0, SensorBits = 8
        };

        /// <summary>
        /// Placeholder for BinSense format.
        /// Update Shift/Bits values once the actual encoding is confirmed.
        /// </summary>
        public static readonly UserDataFormat BinSense = new UserDataFormat
        {
            Name        = "BinSense (verify layout)",
            BinShift    = 8, BinBits    = 8,
            CableShift  = 0, CableBits  = 0,    // fill in once confirmed
            SensorShift = 0, SensorBits = 8
        };

        public static List<UserDataFormat> BuiltIn => new List<UserDataFormat>
            { BinWatch, Opticable, BinSense };
    }
}
