using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Model
{
    public class DeviceLocation
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public List<Device> DevicesInLocation { get; set; } = new List<Device>();
    }
}
