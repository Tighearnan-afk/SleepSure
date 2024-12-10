using SQLite;

namespace SleepSure.Model
{
    [Table("vibrationsensor")]
    public class VibrationSensor : Sensor
    {
        public bool VibrationDetected { get; set; }

        public VibrationSensor() { }
        public VibrationSensor(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            VibrationDetected = false;
        }
    }
}
