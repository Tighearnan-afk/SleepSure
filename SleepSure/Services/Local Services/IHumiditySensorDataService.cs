using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IHumiditySensorDataService
    {
        public Task<List<HumiditySensor>> GetHumiditySensorsAsync(bool isInDemoMode);
        public Task AddHumiditySensorAsync(string name, string description, int deviceLocationId);
        public Task DeleteHumiditySensorAsync(HumiditySensor humiditySensor);
        public Task SyncHumiditySensorsAsync();
        public Task UpdateHumiditySensorAsync(HumiditySensor humiditySensor);
    }
}
