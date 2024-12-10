namespace SleepSure.WebAPI.Models
{
    public class HumiditySensor : Sensor
    {
        public int Humidity { get; set; }

        public HumiditySensor() { }

        public HumiditySensor(string name, string description, int batteryLife, int temperature, int humidity, int deviceLocationId)
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            Humidity = humidity;
        }
    }
}
