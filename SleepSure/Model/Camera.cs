using SQLite;

namespace SleepSure.Model
{
    [Table("camera")]
    public class Camera : BaseDevice
    {
        public Camera() 
        { 
        }

        public Camera(string name, string description, int deviceLocationId) 
        {
            Name = name;
            Description = description;
            DeviceLocationId = deviceLocationId;
            PowerStatus = true;
            OnOrOff = "On";
        }
    }
}
