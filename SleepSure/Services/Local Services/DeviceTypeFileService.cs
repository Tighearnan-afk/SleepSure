using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public class DeviceTypeFileService : IDeviceTypeService
    {
        //A list that stores the device types retrieved from a local JSON file
        public List<Model.DeviceType> _deviceTypes = [];

        public DeviceTypeFileService() 
        {
            
        }

        /// <summary>
        /// The GetTypesAsync method returns a list of device types retrieved from a local JSON file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<Model.DeviceType>> GetTypesAsync()
        {
            try
            {
                if (_deviceTypes.Count > 0)
                    return _deviceTypes;

                //Load the JSON data from the local file using a stream and stream reader
                using var stream = await FileSystem.OpenAppPackageFileAsync("DeviceTypes.json");
                using var streamreader = new StreamReader(stream);
                var contents = await streamreader.ReadToEndAsync();
                //Deserialise the JSON data and store it in the device type list
                _deviceTypes = JsonSerializer.Deserialize<List<Model.DeviceType>>(contents);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //Return the list of device types
            return _deviceTypes;
        }
    }
}
