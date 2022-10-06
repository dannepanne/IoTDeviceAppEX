using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.ScreenWriterEX.Models
{
    internal class DeviceItem
    {
        public string DeviceId { get; set; }
        public int Interval { get; set; }
        public string DeviceConnectionString { get; set; }
        public string DeviceName { get; set; } = "";
    }
}
