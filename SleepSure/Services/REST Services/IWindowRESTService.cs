using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IWindowRESTService
    {
        public Task<List<WindowSensor>> RefreshWindowSensorsAsync();
        public Task SaveCameraAsync(WindowSensor windowSensor, bool isNewWindowSensor);
        public Task DeleteWindowSensorAsync(int id);
    }
}
