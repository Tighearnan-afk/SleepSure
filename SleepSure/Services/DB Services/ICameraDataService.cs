using SleepSure.Model;

namespace SleepSure.Services
{
    public interface ICameraDataService
    {
        public Task<List<Camera>> GetCamerasAsync();
        public Task AddCameraAsync();
    }
}
