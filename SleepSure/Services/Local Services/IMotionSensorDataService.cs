using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IMotionSensorDataService
    {
        public Task<List<MotionSensor>> GetMotionSensorsAsync(bool isInDemoMode);
        public Task AddMotionSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId);
        public Task DeleteMotionSensorAsync(MotionSensor motionSensor);
        public Task SyncMotionSensorsAsync();
        public Task UpdateMotionSensorAsync(MotionSensor motionSensor);
    }
}
