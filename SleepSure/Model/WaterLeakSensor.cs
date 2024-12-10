using SQLite;

namespace SleepSure.Model
{
    [Table("waterleaksensor")]
    public class WaterLeakSensor : Sensor
    {
        public bool LeakDetected { get; set; }

        public WaterLeakSensor() { }

        public WaterLeakSensor(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            LeakDetected = false;
        }
    }
}
