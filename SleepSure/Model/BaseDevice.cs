using SQLite;

namespace SleepSure.Model
{
    [Table("device")]
    public class BaseDevice
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool PowerStatus { get; set; }
        public int DeviceLocationId { get; set; }

        public string OnOrOff { get; set; }

        public BaseDevice() { }
    }
}
