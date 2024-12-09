namespace SleepSure.WebAPI.Models
{
    public class Sensor : BaseDevice
    {
        public int BatteryLife { get; set; }
        public string Temperature { get; set; }
    }
}
