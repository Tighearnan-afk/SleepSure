using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IWaterLeakSensorDataService
    {
        public Task<List<WaterLeakSensor>> GetWaterLeakSensorsAsync(bool isInDemoMode);
        public Task AddWaterLeakSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId);
        public Task DeleteWaterLeakSensorAsync(WaterLeakSensor waterLeakSensor);
        public Task SyncWaterLeakSensorsAsync();
        public Task UpdateWaterLeakSensorAsync(WaterLeakSensor waterLeakSensor);
    }
}
