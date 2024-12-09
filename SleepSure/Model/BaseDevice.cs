using SQLite;

namespace SleepSure.Model
{
    [Table("device")]
    public class BaseDevice
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
        public bool PowerStatus { get; set; }
        public int DeviceLocationId { get; set; }

        public BaseDevice() { }
    }
}
