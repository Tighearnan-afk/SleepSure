using SQLite;

namespace SleepSure.Model
{
    [Table("device")]
    public class Device
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Location { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }

        public Device() { }

        public Device(string name, string location, string description)
        {
            Name = name;
            Location = location;
            Description = description;
        }
    }
}
