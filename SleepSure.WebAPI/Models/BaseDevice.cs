namespace SleepSure.WebAPI.Models
{
    public class BaseDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool PowerStatus { get; set; }
        public int DeviceLocationId { get; set; }
    }
}
