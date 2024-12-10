using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IVibrationSensorDataService
    {
        public Task<List<VibrationSensor>> GetVibrationSensorsAsync(bool isInDemoMode);
        public Task AddVibrationSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId);
        public Task DeleteVibrationSensorAsync(VibrationSensor vibrationSensor);
        public Task SyncVibrationSensorsAsync();
        public Task UpdateVibrationSensorAsync(VibrationSensor vibrationSensor);
    }
}
