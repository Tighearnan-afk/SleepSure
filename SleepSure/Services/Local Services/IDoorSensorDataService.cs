using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IDoorSensorDataService
    {
        public Task<List<DoorSensor>> GetDoorSensorsAsync(bool isInDemoMode);
        public Task AddDoorSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId);
        public Task DeleteDoorSensorAsync(DoorSensor doorSensor);
        public Task SyncDoorSensorsAsync();
        public Task UpdateDoorSensorAsync(DoorSensor doorSensor);
    }
}
