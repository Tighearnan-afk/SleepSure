using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ITemperatureSensorRESTService
    {
        public Task<List<TemperatureSensor>> RefreshTemperatureSensorsAsync();
        public Task SaveTemperatureSensorAsync(TemperatureSensor temperatureSensor, bool isNewTemperatureSensor);
        public Task DeleteTemperatureSensorAsync(int id);
    }
}
