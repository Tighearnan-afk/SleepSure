using SQLite;

namespace SleepSure.Model
{
    [Table("light")]
    public class Light : BaseDevice
    {
        public int Brightness { get; set; }

        public Light() { }

        public Light(string name, string description, int brightness, int deviceLocationId)
        {
            Name = name;
            Description = description;
            Brightness = brightness;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
        }
    }
}
