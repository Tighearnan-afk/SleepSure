using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Model
{
    [Table("sensor")]
    public class Sensor : BaseDevice
    {
        public int BatteryLife { get; set; }
        public int Temperature { get; set; }

        public Sensor() { }
    }
}
