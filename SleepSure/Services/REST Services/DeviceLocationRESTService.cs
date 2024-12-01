using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SleepSure.Services.REST_Services
{
    public class DeviceLocationRESTService : IDeviceLocationRESTService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public List<DeviceLocation> Locations { get; private set; }

        public DeviceLocationRESTService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<DeviceLocation>> RefreshLocationsAsync()
        {
            Locations = [];
            string locationEndPoint = string.Concat(Constants.RestUrl, $"devicelocations/{{0}}");

            Uri uri = new Uri(string.Format(locationEndPoint, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Locations = JsonSerializer.Deserialize<List<DeviceLocation>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Locations;
        }

        public Task SaveLocationAsync(DeviceLocation location, bool isNewLocation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLocationAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
