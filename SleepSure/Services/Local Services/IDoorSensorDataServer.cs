﻿using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IDoorSensorDataServer
    {
        public Task<List<DoorSensor>> GetDoorSensorsAsync(bool isInDemoMode);
        public Task AddDoorSensorAsync(string name, string description, int deviceLocationId);
        public Task DeleteDoorSensorAsync(DoorSensor doorSensor);
        public Task SyncDoorSensorsAsync();
        public Task UpdateDoorSensorAsync(DoorSensor doorSensor);
    }
}
