using SleepSure.Model;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SleepSure.Services
{
    public class DeviceLocationRESTService : IDeviceLocationRESTService
    {
        HttpClient _client;

        JsonSerializerOptions _serializerOptions;
        //List of locations retrieved from the REST API
        public List<DeviceLocation> Locations { get; private set; }

        public DeviceLocationRESTService()
        {
            //Retrieved from "https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-9.0&viewFallbackFrom=net-maui-7.0"
            //Needed to allow the android emulator to connect to the REST API over HTTPS
            var handler = new HttpClientHandler();

#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
#endif

            _client = new HttpClient(handler);
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

        public async Task SaveLocationAsync(DeviceLocation location, bool isNewLocation)
        {
            string locationEndPoint = string.Concat(Constants.RestUrl, $"devicelocations/{{0}}");

            Uri uri = new Uri(string.Format(locationEndPoint, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize(location, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (isNewLocation)
                    response = await _client.PostAsync(uri, content);
                else
                    response = await _client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tLocation successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task DeleteLocationAsync(int id)
        {
            string locationEndPoint = string.Concat(Constants.RestUrl, $"devicelocations/{{0}}");

            Uri uri = new Uri(string.Format(locationEndPoint, id));

            try
            {

                HttpResponseMessage response = null;
                
                response = await _client.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tLocation successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
