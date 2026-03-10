using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BinWatch.Models
{
    public class Module
    {
        [Key]
        public string MacAddress { get; set; }      // "AA:BB:CC:DD:EE:FF"

        public byte ModuleId { get; set; }          // 0 = unregistered
        public string Name { get; set; }
        public string LastKnownIp { get; set; }
        public DateTime? LastSeen { get; set; }
        public string Status { get; set; }          // Online, Offline, Unregistered
        public ushort FirmwareVersion { get; set; } // InoID

        public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
    }
}
