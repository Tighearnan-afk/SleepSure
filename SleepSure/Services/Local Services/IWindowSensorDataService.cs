using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IWindowSensorDataService
    {
        public Task<List<WindowSensor>> GetWindowSensorsAsync(bool isInDemoMode);
        public Task AddWindowSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId);
        public Task DeleteWindowSensorAsync(WindowSensor windowSensor);
        public Task SyncWindowSensorsAsync();
        public Task UpdateWindowSensorAsync(WindowSensor windowSensor);
    }
}
