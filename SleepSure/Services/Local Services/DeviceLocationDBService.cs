using Microsoft.Maui.Devices.Sensors;
using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;


namespace SleepSure.Services
{
    public class DeviceLocationDBService : IDeviceLocationDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for cameras
        IDeviceLocationRESTService _locationRESTService;
        //Local data service for cameras
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

        //Contains a string that contains the path to the database on the device
        readonly string _dbPath;

        public string StatusMessage;

        private bool _isInDemoMode;
        public DeviceLocationDBService(string dbPath, IDeviceLocationRESTService locationRESTService, ICameraDataService cameraDataService)
        {
            _dbPath = dbPath;
            _locationRESTService = locationRESTService;
            _cameraDataService = cameraDataService;
            _internet = Connectivity.Current.NetworkAccess;
        }

        /// <summary>
        /// Initialise the connection to the database
        /// </summary>

        public async Task Init()
        {
            //If the database has already been created this method does nothing
            if (_connection is not null)
                return;
            //Creates a connection using the database path and its flags
            _connection = new SQLiteAsyncConnection(_dbPath, _dbFLags);
            //Creates the device location table
            var result = await _connection.CreateTableAsync<DeviceLocation>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<DeviceLocation>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default locations stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default locations from a local JSON file
                await GetJSONLocationsAsync();
                //Iterates through the list of default locations
                foreach (var locations in JSONLocations)
                {
                    //Inserts the default location into the SQLite database
                    await _connection.InsertAsync(locations);
                }
            }
        }

        /// <summary>
        /// The AddLocationAsync method inserts a new location into the local SQLite database
        /// </summary>
        /// <param name="locationName"></param>
        
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

        /// <summary>
        /// The GetLocationsAsync method returns a list of locations retrieved from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of locations</returns>
        public async Task<List<DeviceLocation>> GetLocationsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the demo mode flag based on if the application is in demo mode or not
                _isInDemoMode = isInDemoMode;
                //Ensures the connection to the database is initialised
                await Init();
                //Returns a list locations retrieved from the local SQLite database
                return await _connection.Table<DeviceLocation>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get locations from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<DeviceLocation>();
        }
        /// <summary>
        /// The GetJSONLocationsAsync method deserialises a list of default locations into the JSONLocations list
        /// </summary>
        public async Task GetJSONLocationsAsync()
        {
            //Ensures the JSON locations list is empty
            if (JSONLocations.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoDeviceLocations.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            //Deserialises the contents of the demo locations JSON file into the JSON locations list
            JSONLocations = JsonSerializer.Deserialize<List<DeviceLocation>>(content);
        }

        /// <summary>
        /// The SyncLocationAsync method synchronises the locations between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncLocationsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the locations present in the Locations list
                RESTLocations = await _locationRESTService.RefreshLocationsAsync();
                //Refreshes the locations present in the LocalLocations list
                LocalLocations = await GetLocationsAsync(_isInDemoMode);
                //Iterates through the LocalLocations list
                foreach(var localLocation in LocalLocations)
                {
                    //Checks if any location present in the SQLite database is present in the REST API in memory database
                    if(!RESTLocations.Any(l => l.Id == localLocation.Id && l.LocationName == localLocation.LocationName))
                    {
                        //If the location is not present calls the LocationRESTServices SaveLocationAsync method which will post the location to the REST API
                        await _locationRESTService.SaveLocationAsync(localLocation, true);
                    }
                }
                //Iterates through the RESTLocations list
                foreach(var restLocation in RESTLocations)
                {
                    //Checks if any location present in the REST API in memory database is not present in the SQLite database 
                    if(!LocalLocations.Any(l => l.Id == restLocation.Id && l.LocationName == restLocation.LocationName))
                    {
                        //If the location is not present it is inserted into the local SQLite database
                        await _connection.InsertAsync(restLocation);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The RemoveLocationAsync method removes a location from both the local SQLite database and REST API alongside its associated devices
        /// </summary>
        /// <param name="location"></param>

        public async Task RemoveLocationAsync(DeviceLocation location)
        {
            //Stores the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the conenction to the SQLite database is created
                await Init();

                //A temporary lsit of cameras
                List<Camera> TempCameras = [];
                //A list of cameras that are associated with the current room
                List<Camera> AssociatedCameras = [];
                //Retrieves a list of all cameras and inserts them into the temporary list
                TempCameras = await _cameraDataService.GetCamerasAsync(_isInDemoMode);

                //Iterates through the temporary list
                foreach (var camera in TempCameras)
                {
                    //Checks if the camera has the current location is associated with the camera
                    if (camera.DeviceLocationId == location.Id)
                    {
                        //If it is then adds the camera to the associated camera list
                        AssociatedCameras.Add(camera);
                    }
                }

                //Iterates through the associated camera list
                foreach (var camera in AssociatedCameras)
                {
                    //Deletes the camera from the SQLite database
                    await _cameraDataService.DeleteCameraAsync(camera);
                }

                //Deletes the location from the SQLite database and stores the result code
                result = await _connection.DeleteAsync(location);
                //Deletes the location from the REST API, requires a cast to int as the model has a nullable id to allow sqlite to increment it correctly
                await _locationRESTService.DeleteLocationAsync((int)location.Id);
            }
            catch(Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateLocationsAsync method updates the name of a location
        /// </summary>
        /// <param name="location"></param>
        public async Task UpdateLocationAsync(DeviceLocation location)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Updates the location name and records the result
                result = await _connection.UpdateAsync(location);
                
                //await _locationRESTService.DeleteLocationAsync((int)location.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
