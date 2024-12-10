
namespace SleepSure.Model
{
    public class TemperatureSensor: Sensor
    {
        public TemperatureSensor() { }

        public TemperatureSensor(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
        }
    }
}
