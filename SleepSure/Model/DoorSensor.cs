
namespace SleepSure.Model
{
    public class DoorSensor : Sensor
    {
        public bool IsOpen { get; set; }

        public DoorSensor() { }

        public DoorSensor(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            IsOpen = false;
        }
    }
}
