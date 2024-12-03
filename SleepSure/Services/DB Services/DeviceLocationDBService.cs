using SleepSure.Model;
using SleepSure.Services.REST_Services;
using SQLite;
using System.Diagnostics;
using System.Text.Json;


namespace SleepSure.Services.DB_Services
{
    public class DeviceLocationDBService : IDeviceLocationDataService
    {
        //REST API service for cameras
        IDeviceLocationRESTService _locationRESTService;

        ICameraDataService _cameraDataService;
        //List of locations retrieved from the REST API
        public List<DeviceLocation> RESTLocations { get; private set; } = [];
        //List of locations retrieved from a local JSON file
        public List<DeviceLocation> JSONLocations { get; private set; } = [];
        //List of cameras in the local SQLite database
        public List<DeviceLocation> LocalLocations = [];
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
        public DeviceLocationDBService(string dbPath, IDeviceLocationRESTService locationRESTService, ICameraDataService cameraDataService)
        {
            _dbPath = dbPath;
            _locationRESTService = locationRESTService;
            _cameraDataService = cameraDataService;
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
            var result = await _connection.CreateTableAsync<DeviceLocation>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<DeviceLocation>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default locations stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                await GetJSONLocationsAsync();
                foreach (var locations in JSONLocations)
                {
                    await _connection.InsertAsync(locations);
                }
            }
        }


        public async Task AddLocationAsync(string locationName)
        {
            int result = 0;
            try
            {
                await Init();
                result = await _connection.InsertAsync(new DeviceLocation(locationName));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add location. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        public async Task<List<DeviceLocation>> GetLocationsAsync(bool isInDemoMode)
        {
            try
            {
                _isInDemoMode = isInDemoMode;
                await Init();
                return await _connection.Table<DeviceLocation>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get locations from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<DeviceLocation>();
        }

        public async Task GetJSONLocationsAsync()
        {
            if (JSONLocations.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoDeviceLocations.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONLocations = JsonSerializer.Deserialize<List<DeviceLocation>>(content);
        }

        //Method to sync devices present on the SQLite database with the REST API
        public async Task SyncLocationsAsync()
        {
            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the devices present in the Locations list
                RESTLocations = await _locationRESTService.RefreshLocationsAsync();
                //Refreshes the devices present in the LocalLocations list
                LocalLocations = await GetLocationsAsync(_isInDemoMode);
                //Iterates through the LocalLocation list
                foreach(var localLocation in LocalLocations)
                {
                    //Checks if any location present in the SQLite database is present in the REST API in memory database
                    if(!RESTLocations.Any(l => l.Id == localLocation.Id && l.LocationName == localLocation.LocationName))
                    {
                        //If the device is not present calls the LocationRESTServices SaveLocationAsync method which will post the location to the REST API
                        await _locationRESTService.SaveLocationAsync(localLocation, true);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        public async Task RemoveLocationAsync(DeviceLocation location)
        {
            int result = 0;
            try
            {
                await Init();

                List<Camera> TempCameras = [];
                List<Camera> AssociatedCameras = [];
                TempCameras = await _cameraDataService.GetCamerasAsync();

                foreach (var camera in TempCameras)
                {
                    if (camera.DeviceLocationId == location.Id)
                    {
                        AssociatedCameras.Add(camera);
                    }
                }
                
                foreach (var camera in AssociatedCameras)
                {
                    await _cameraDataService.DeleteCameraAsync(camera);
                }

                result = await _connection.DeleteAsync(location);
            }
            catch(Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
