using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class CameraDBDataService : ICameraDataService
    {
        //REST API service for cameras
        ICameraRESTService _cameraRESTService;
        //List of cameras retrieved from the REST API
        public List<Camera> RESTCameras { get; private set; } = [];
        //List of locations retrieved from a local JSON file
        public List<Camera> JSONCameras { get; private set; } = [];
        //List of cameras in the local SQLite database
        public List<Camera> LocalCameras { get; private set; } = [];
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

        public CameraDBDataService(string dbPath, ICameraRESTService cameraRESTService)
        {
            _dbPath = dbPath;
            _cameraRESTService = cameraRESTService;
        }

        /// <summary>
        /// Initialise the connection to the database
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Init()
        {
            //If the database has already been created this method does nothing
            if (_connection is not null)
                return;
            //Creates a connection using the database path and its flags
            _connection = new SQLiteAsyncConnection(_dbPath, _dbFLags);
            //Creates the device table
            var result = await _connection.CreateTableAsync<Camera>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<Camera>().CountAsync();
            //If no rows exist seeds the SQLite database with data fetched from the REST API
            if(tableData == 0 && _isInDemoMode)
            {
                await GetJSONCamerasAsync();
                foreach (var camera in JSONCameras)
                {
                    await _connection.InsertAsync(camera);
                }
            }
        }


        public async Task AddCameraAsync(string name, string description, int deviceLocationId)
        {
            int result = 0;
            try
            {
                await Init();
                result = await _connection.InsertAsync(new Camera(name, description, deviceLocationId));
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

        //Method to sync cameras between the SQLite database and the REST API
        public async Task SyncCamerasAsync()
        {
            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the devices present in the Locations list
                RESTCameras = await _cameraRESTService.RefreshCamerasAsync();
                //Refreshes the devices present in the LocalLocations list
                LocalCameras = await GetCamerasAsync(_isInDemoMode);
                //Iterates through the LocalLocations list
                foreach (var localCamera in LocalCameras)
                {
                    //Checks if any location present in the SQLite database is present in the REST API in memory database
                    if (!RESTCameras.Any(l => l.Id == localCamera.Id))
                    {
                        //If the device is not present calls the LocationRESTServices SaveLocationAsync method which will post the location to the REST API
                        await _cameraRESTService.SaveCameraAsync(localCamera, true);
                    }
                }
                //Iterates through the RESTLocations list
                foreach (var restCamera in RESTCameras)
                {
                    //Checks if any location present in the REST API in memory database is not present in the SQLite database and inserts it
                    if (!LocalCameras.Any(l => l.Id == restCamera.Id))
                    {
                        await _connection.InsertAsync(restCamera);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        public async Task DeleteCameraAsync(Camera camera)
        {
            try
            {
                await Init();
                await _connection.DeleteAsync(camera);
            }
            catch(Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
