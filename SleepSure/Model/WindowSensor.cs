using SQLite;

namespace SleepSure.Model
{
    [Table("windowsensor")]
    public class WindowSensor : Sensor
    {
        public bool IsOpen { get; set; }

        public WindowSensor() { }

        public WindowSensor(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            OnOrOff = "On";
            IsOpen = false;
        }
    }
}
