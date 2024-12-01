using SleepSure.Model;

namespace SleepSure.Services.REST_Services
{
    public interface IDeviceLocationRESTService
    {
        public Task<List<DeviceLocation>> RefreshLocationsAsync();
        public Task SaveLocationAsync(DeviceLocation location, bool isNewLocation);
        public Task DeleteLocationAsync(int id);
    }
}
