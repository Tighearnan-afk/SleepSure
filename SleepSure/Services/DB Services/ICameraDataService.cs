using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ICameraDataService
    {
        public Task<List<Camera>> GetCamerasAsync(bool isInDemoMode);
        public Task AddCameraAsync();
        public Task DeleteCameraAsync(Camera camera);
    }
}
