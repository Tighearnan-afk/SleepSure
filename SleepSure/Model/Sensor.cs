using SQLite;

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
