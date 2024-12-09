using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Model
{
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
        }
    }
}
