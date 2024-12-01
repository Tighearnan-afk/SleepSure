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
        [MaxLength(30)]
        public string Location { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
        public int LocationId { get; set; }

        public BaseDevice() { }

        public BaseDevice(string name, string location, string description, int locationId)
        {
            Name = name;
            Location = location;
            Description = description;
            LocationId = locationId;
        }
    }
}
