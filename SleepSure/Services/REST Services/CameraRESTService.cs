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
            _client = new HttpClient();
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

            Uri uri = new Uri(string.Format(cameraEndPoint, string.Empty));

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

        public Task DeleteCameraAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
