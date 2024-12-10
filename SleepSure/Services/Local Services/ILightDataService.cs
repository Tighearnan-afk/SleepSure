using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ILightDataService
    {
        public Task<List<Light>> GetLightsAsync(bool isInDemoMode);
        public Task AddLightAsync(string name, string description, int brightness, int deviceLocationId);
        public Task DeleteLightAsync(Light light);
        public Task SyncLightsAsync();
        public Task UpdateLightAsync(Light light);
    }
}
