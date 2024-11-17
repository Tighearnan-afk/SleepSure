using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public interface IDeviceDataService
    {
        public Task<List<Model.Device>> GetDevicesAsync();
        public Task AddDeviceAsync();
    }
}
