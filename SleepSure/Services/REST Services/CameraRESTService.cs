using Microsoft.VisualBasic;
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
    public class CameraRESTService : ICameraRESTService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public List<Camera> Cameras { get; private set; }

        public CameraRESTService()
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

        public async Task<List<Camera>> RefreshCamerasAsync()
        {
            Cameras = [];
            string cameraEndPoint = string.Concat(Constants.RestUrl,$"cameras/{{0}}");

            Uri uri = new Uri(string.Format(cameraEndPoint, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Cameras = JsonSerializer.Deserialize<List<Camera>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Cameras;
        }

        public async Task SaveCameraAsync(Camera camera, bool isNewCamera)
        {
            string cameraEndPoint = string.Concat(Constants.RestUrl, $"cameras/{{0}}");

            Uri uri;
            if (isNewCamera)
                uri = new Uri(string.Format(cameraEndPoint, string.Empty));
            else
                uri = new Uri(string.Format(cameraEndPoint, camera.Id));

            try
            {
                string json = JsonSerializer.Serialize(camera, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if(isNewCamera)
                    response = await _client.PostAsync(uri, content);
                else
                    response = await _client.PutAsync(uri, content);

                if(response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tCamera successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task DeleteCameraAsync(int id)
        {
            string locationEndPoint = string.Concat(Constants.RestUrl, $"cameras/{{0}}");

            Uri uri = new Uri(string.Format(locationEndPoint, id));

            try
            {

                HttpResponseMessage response = null;

                response = await _client.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\Camera deleted saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
