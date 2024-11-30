using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public interface ICameraRESTService
    {
        public Task<List<Camera>> RefreshCamerasAsync();
        public Task SaveCameraAsync(Camera camera, bool isNewCamera);
        public Task DeleteCameraAsync(int id);
    }
}
