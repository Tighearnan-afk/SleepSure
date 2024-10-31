using SleepSure.Model;
using System.Text.Json;

namespace SleepSure.Services
{
    public class DeviceService
    {
        public List<Model.Device> _deviceList = new List<Model.Device>();
        public DeviceService() 
        {
            
        }
        /// <summary>
        /// The GetDeviceFileAsync task reads a list of devices from a local JSON file
        /// </summary>
        /// <returns>A list of device objects</returns>
        public async Task<List<Model.Device>> GetDevicesFileAsync()
        {
            if (_deviceList.Count > 0)
                return _deviceList;

            //Load the json data from the file
            using var stream = await FileSystem.OpenAppPackageFileAsync("UserDevices.json");
            using var streamreader = new StreamReader(stream);
            var contents = await streamreader.ReadToEndAsync();
            _deviceList = JsonSerializer.Deserialize<List<Model.Device>>(contents);

            //Return the list of devices
            return _deviceList;
        }
    }
}
