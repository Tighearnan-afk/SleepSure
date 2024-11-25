using SQLite;


namespace SleepSure.Model
{
    [Table("windowdoorsensor")]
    public class WindowDoorSensor : Sensor
    {
        public string Type { get; set; }
        public bool IsOpen { get; set; }
    }
}
