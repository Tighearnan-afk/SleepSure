﻿using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IDeviceLocationRESTService
    {
        public Task<List<DeviceLocation>> RefreshLocationsAsync();
        public Task SaveLocationAsync(DeviceLocation location, bool isNewLocation);
        public Task DeleteLocationAsync(int id);
    }
}
