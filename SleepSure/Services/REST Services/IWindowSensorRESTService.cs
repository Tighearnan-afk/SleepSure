using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IWindowSensorRESTService
    {
        public Task<List<WindowSensor>> RefreshWindowSensorsAsync();
        public Task SaveWindowSensorAsync(WindowSensor windowSensor, bool isNewWindowSensor);
        public Task DeleteWindowSensorAsync(int id);
    }
}
