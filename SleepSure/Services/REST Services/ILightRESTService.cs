using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ILightRESTService
    {
        public Task<List<Light>> RefreshLightsAsync();
        public Task SaveLightAsync(Light light, bool isNewLight);
        public Task DeleteLightAsync(int id);
    }
}
