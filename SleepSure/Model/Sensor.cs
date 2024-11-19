using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Model
{
    [Table("sensor")]
    public class Sensor : Device
    {
        public int BatteryLife { get; set; }

        public Sensor() { }

        public Sensor (string name, string location, string description,int batteryLife)
        {
            Name = name;
            Location = location;
            Description = description;
            BatteryLife = batteryLife;
        }
    }
}
