using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ICameraDataService
    {
        public Task<List<Camera>> GetCamerasAsync(bool isInDemoMode);
        public Task AddCameraAsync(string name, string description, int deviceLocationId);
        public Task DeleteCameraAsync(Camera camera);
        public Task SyncCamerasAsync();
        public Task UpdateCameraAsync(Camera camera);
    }
}
