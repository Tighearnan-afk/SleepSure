using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IVibrationSensorRESTService
    {
        public Task<List<VibrationSensor>> RefreshVibrationSensorsAsync();
        public Task SaveVibrationSensorAsync(VibrationSensor vibrationSensor, bool isNewVibrationSensor);
        public Task DeleteVibrationSensorAsync(int id);
    }
}
