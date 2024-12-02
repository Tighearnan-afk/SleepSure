namespace SleepSure.WebAPI.Models
{
    public class BaseDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; } //DELETE
        public string Description { get; set; }
        public int DeviceLocationId { get; set; }
        //public DeviceLocation DeviceLocation { get; set; }
    }
}
