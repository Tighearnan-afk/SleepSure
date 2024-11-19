using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ISensorDataService
    {
        public Task<List<Sensor>> GetSensorsAsync();
        public Task AddSensorAsync();
    }
}
