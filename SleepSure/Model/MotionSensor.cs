using SQLite;

namespace SleepSure.Model
{
    [Table("motionsensor")]
    public class MotionSensor : Sensor
    {
        public bool MotionDetected { get; set; }

        public MotionSensor() { }

        public MotionSensor(string name, string description, int batteryLife, int temperature, int deviceLocationId) 
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            OnOrOff = "On";
            MotionDetected = false;
        }
    }
}
