using SQLite;

namespace SleepSure.Model
{
    [Table("devicelocation")]
    public class DeviceLocation
    {
        [PrimaryKey,AutoIncrement]
        public int? Id { get; set; }
        public string LocationName { get; set; }
        public DeviceLocation() { }
        
        public DeviceLocation(string name, string description)
        {
            LocationName = name;
        }
    }
}
