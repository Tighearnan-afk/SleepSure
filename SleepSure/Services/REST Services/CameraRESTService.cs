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

        public Task SaveCameraAsync(Camera camera, bool isNewCamera)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCameraAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
