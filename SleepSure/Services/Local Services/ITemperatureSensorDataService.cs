using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ITemperatureSensorDataService
    {
        public Task<List<TemperatureSensor>> GetTemperatureSensorsAsync(bool isInDemoMode);
        public Task AddTemperatureSensorAsync(string name, string description, int deviceLocationId);
        public Task DeleteTemperatureSensorAsync(TemperatureSensor temperatureSensor);
        public Task SyncTemperatureSensorsAsync();
        public Task UpdateTemperatureSensorAsync(TemperatureSensor temperatureSensor);
    }
}
