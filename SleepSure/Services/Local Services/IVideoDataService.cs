using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IVideoDataService
    {
        public Task<List<Video>> GetVideosAsync(bool isInDemoMode);
        public Task AddVideoAsync(int cameraId);
        public Task DeleteVideoAsync(Video video);
    }
}
