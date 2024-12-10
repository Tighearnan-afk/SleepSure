using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IDoorSensorRESTService
    {
        public Task<List<DoorSensor>> RefreshDoorSensorsAsync();
        public Task SaveDoorSensorAsync(DoorSensor doorSensor, bool isNewDoorSensor);
        public Task DeleteDoorSensorAsync(int id);
    }
}
