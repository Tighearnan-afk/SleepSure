using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IMotionSensorRESTService
    {
        public Task<List<MotionSensor>> RefreshMotionSensorsAsync();
        public Task SaveMotionSensorAsync(MotionSensor motionSensor, bool isNewSensor);
        public Task DeleteMotionSensorAsync(int id);
    }
}
