using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IWaterLeakSensorRESTService
    {
        public Task<List<WaterLeakSensor>> RefreshWaterLeakSensorsAsync();
        public Task SaveWaterLeakSensorAsync(WaterLeakSensor waterLeakSensor, bool isNewWaterLeakSensor);
        public Task DeleteWaterLeakSensorAsync(int id);
    }
}
