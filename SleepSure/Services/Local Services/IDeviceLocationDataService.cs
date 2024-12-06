using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public interface IDeviceLocationDataService
    {
        public Task<List<DeviceLocation>> GetLocationsAsync(bool isInDemoMode);
        public Task AddLocationAsync(string location);
        public Task RemoveLocationAsync(DeviceLocation location);
        public Task SyncLocationsAsync();
    }
}
