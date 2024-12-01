namespace SleepSure.WebAPI.Models
{
    public class DeviceLocation
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public List<Camera> Cameras { get; set; }
    }
}
