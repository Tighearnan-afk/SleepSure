using SQLite;

namespace SleepSure.Model
{
    [Table("Alarm")]
    public class Alarm
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public string DeviceName { get; set; }
        public DateTime DateTime { get; set; }

        public Alarm() { }

        public Alarm(string eventName, string eventDescription, string deviceName, DateTime dateTime)
        {
            EventName = eventName;
            EventDescription = eventDescription;
            DeviceName = deviceName;
            DateTime = dateTime;
        }
    }
}
