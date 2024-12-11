using SleepSure.Model;

namespace SleepSure.Services
{
    public interface IAlarmDataService
    {
        public Task<List<Alarm>> GetAlarmsAsync();
        public Task CreateAlarmAsync(string eventName, string eventDescription, string deviceName, DateTime dateTime);
        public Task RemoveAlarmAsync(Alarm alarm);
    }
}
