using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IWindowSensorDataService
    {
        public Task<List<WindowSensor>> GetWindowSensorsAsync(bool isInDemoMode);
        public Task AddWindowSensor(string name, string description, int deviceLocationId);
        public Task DeleteWindowSensor(WindowSensor windowSensor);
        public Task SyncWindowSensors();
        public Task UpdateWindowSensorAsync(WindowSensor windowSensor);
    }
}
