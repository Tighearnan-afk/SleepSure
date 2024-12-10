using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IHumiditySensor
    {
        public Task<List<HumiditySensor>> RefreshHumiditySensorsAsync();
        public Task SaveHumiditySensorAsync(HumiditySensor humiditySensor, bool isNewHumiditySensor);
        public Task DeleteHumiditySensorAsync(int id);
    }
}
