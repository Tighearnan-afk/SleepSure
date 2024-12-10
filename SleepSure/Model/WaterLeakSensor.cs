namespace SleepSure.Model
{
    public class WaterLeakSensor : Sensor
    {
        public bool LeakDetected;

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
