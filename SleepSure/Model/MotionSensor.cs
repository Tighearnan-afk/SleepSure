﻿
namespace SleepSure.Model
{
    public class MotionSensor : Sensor
    {
        public bool MotionDetected { get; set; }

        public MotionSensor() { }

        public MotionSensor(string name, string description, int batteryLife, string temperature, int deviceLocationId) 
        {
            Name = name;
            Description = description;
            BatteryLife = batteryLife;
            Temperature = temperature;
            DeviceLocationId = deviceLocationId;
            MotionDetected = false;
        }
    }
}
