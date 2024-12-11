using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class CameraDBDataService : ICameraDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for cameras
        ICameraRESTService _cameraRESTService;
        //A service that allows videos to be created and associated with the current camera
        IVideoDataService _videoDataService;
        //List of cameras retrieved from the REST API
        public List<Camera> RESTCameras { get; private set; } = [];
        //List of locations retrieved from a local JSON file
        public List<Camera> JSONCameras { get; private set; } = [];
        //List of cameras in the local SQLite database
        public List<Camera> LocalCameras { get; private set; } = [];
        //List of videos associated with a camera     
        public List<Video> Videos { get; private set; } = [];
        //SQLite connection
        SQLiteAsyncConnection _connection;

        private const SQLite.SQLiteOpenFlags _dbFLags =
            //Open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            //Create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            //Enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        readonly string _dbPath;

        public string StatusMessage;

        private bool _isInDemoMode;

        public CameraDBDataService(string dbPath, ICameraRESTService cameraRESTService, IVideoDataService videoDataService)
        {
            _dbPath = dbPath;
            _cameraRESTService = cameraRESTService;
            _internet = Connectivity.Current.NetworkAccess;
            _videoDataService = videoDataService;
        }

        /// <summary>
        /// Initialise the connection to the database
        /// </summary> 
        /// 
        public async Task Init()
        {
            //If the database has already been created this method does nothing
            if (_connection is not null)
                return;
            //Creates a connection using the database path and its flags
            _connection = new SQLiteAsyncConnection(_dbPath, _dbFLags);
            //Creates the camera table
            var result = await _connection.CreateTableAsync<Camera>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<Camera>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default cameras stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default cameras from a local JSON file
                await GetJSONCamerasAsync();
                //Iterates through the list of default cameras
                foreach (var camera in JSONCameras)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(camera);
                }
                //Uses the GetVideosAsyc method to create the default videos associated with the demo cameras
                await _videoDataService.GetVideosAsync(_isInDemoMode);
            }
        }

        public async Task AddCameraAsync(string name, string description, int deviceLocationId)
        {
            int result = 0;
            try
            {
                await Init();
                Camera newCamera = new Camera(name, description, deviceLocationId);
                result = await _connection.InsertAsync(newCamera);
                await _videoDataService.AddVideoAsync((int)newCamera.Id);

                //Call the sync method as the Camera created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncCamerasAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add location. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        public async Task<List<Camera>> GetCamerasAsync(bool isInDemoMode)
        {
            try
            {
                _isInDemoMode = isInDemoMode;
                await Init();
                return await _connection.Table<Camera>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<Camera>();
        }

        public async Task GetJSONCamerasAsync()
        {
            if (JSONCameras.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoCameras.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONCameras = JsonSerializer.Deserialize<List<Camera>>(content);
        }

        /// <summary>
        /// The SyncCamerasAsync method synchronises the cameras between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncCamerasAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the cameras present in the REST cameras list
                RESTCameras = await _cameraRESTService.RefreshCamerasAsync();
                //Refreshes the cameras present in the local cameras list
                LocalCameras = await GetCamerasAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var localCamera in LocalCameras)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTCameras.Any(l => l.Id == localCamera.Id))
                    {
                        //If the camera is not present calls the CameraRESTServices SaveCameraAsync method which will post the location to the REST API
                        await _cameraRESTService.SaveCameraAsync(localCamera, true);
                    }
                }
                //Iterates through the RESTCameras list
                foreach (var restCamera in RESTCameras)
                {
                    //Checks if any camera present in the REST API in memory database is not present in the SQLite database
                    if (!LocalCameras.Any(l => l.Id == restCamera.Id))
                    {
                        //If the camera is not present it is inserted into the local SQLite database
                        await AddCameraAsync(restCamera.Name, restCamera.Description, restCamera.DeviceLocationId);
                    }
                    if (LocalCameras.Any(l => l.Id == restCamera.Id && l.PowerStatus != restCamera.PowerStatus))
                    {
                        //If the camera is turned off in the REST API in memory database turn the camera off in the local SQLite database
                        await UpdateCameraAsync(restCamera);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync cameras. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteCameraAsync method deletes a specified camera from the local SQLite database
        /// </summary>
        /// <param name="camera"></param>
        public async Task DeleteCameraAsync(Camera camera)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                var videos = await _videoDataService.GetVideosAsync(false);

                foreach (var video in videos)
                {
                    if (video.CameraId == camera.Id)
                        Videos.Add(video);
                }

                foreach (var video in Videos)
                    await _videoDataService.DeleteVideoAsync(video);


                //Delete the specified camera
                await _connection.DeleteAsync(camera);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the camera from REST API
                await _cameraRESTService.DeleteCameraAsync((int)camera.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete video from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateCamerAsync method updates the details of the camera
        /// </summary>
        /// <param name="camera"></param>
        public async Task UpdateCameraAsync(Camera camera)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Sets the string property OnOrOff based on the boolean property PowerStatus for display purposes in the location page
                if (camera.PowerStatus)
                    camera.OnOrOff = "On";
                else
                    camera.OnOrOff = "Off";

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(camera);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await _cameraRESTService.SaveCameraAsync(camera, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update camera. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
