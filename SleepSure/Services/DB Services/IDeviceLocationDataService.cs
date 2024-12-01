using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services.DB_Services
{
    public interface IDeviceLocationDataService
    {
        public Task<List<DeviceLocation>> GetLocationsAsync();
        public Task AddLocationAsync(DeviceLocation location);
    }
}
