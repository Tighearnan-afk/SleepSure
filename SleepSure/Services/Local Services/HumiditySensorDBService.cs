using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class HumiditySensorDBService : IHumiditySensorDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for humidity sensors
        IHumiditySensorRESTService _humidityRESTService;

        //List of humidity sensors retrieved from the REST API
        public List<HumiditySensor> RESTHumiditySensors { get; private set; } = [];
        //List of humidity sensors retrieved from a local JSON file
        public List<HumiditySensor> JSONHumiditySensors { get; private set; } = [];
        //List of humidity sensors in the local SQLite database
        public List<HumiditySensor> LocalHumiditySensors { get; private set; } = [];

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

        public HumiditySensorDBService(string dbPath, IHumiditySensorRESTService humiditySensorRESTService)
        {
            _dbPath = dbPath;
            //humidityRESTService = cameraRESTService;
            _internet = Connectivity.Current.NetworkAccess;
            _humidityRESTService = humiditySensorRESTService;
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
            //Creates the humidity sensor table
            var result = await _connection.CreateTableAsync<HumiditySensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<HumiditySensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default humidity sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default humidity sensors from a local JSON file
                await GetJSONHumiditySensorsAsync();
                //Iterates through the list of default cameras
                foreach (var humiditySensor in JSONHumiditySensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(humiditySensor);
                }
            }
        }
        /// <summary>
        /// The AddHumiditySensor method adds a new humidity sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddHumiditySensorAsync(string name, string description, int batteryLife, int temperature, int humidity, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new humidity sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new HumiditySensor(name, description, batteryLife, temperature, humidity, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the HumiditySensor created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncHumiditySensorsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add humidity sensor. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetHumiditySensorsAsync method retrieves a list of all humidity sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of humidity sensors</returns>
        public async Task<List<HumiditySensor>> GetHumiditySensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all humidity sensors in the SQLite database
                return await _connection.Table<HumiditySensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get humidity sensors from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of humidity sensors if an exception occurs
            return new List<HumiditySensor>();
        }
        /// <summary>
        /// The GetJSONHumiditySensorsAsync method deserialises a list of default humidity sensors into the JSONHumiditySensors list
        /// </summary>
        public async Task GetJSONHumiditySensorsAsync()
        {
            if (JSONHumiditySensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoHumiditySensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONHumiditySensors = JsonSerializer.Deserialize<List<HumiditySensor>>(content);
        }

        /// <summary>
        /// The SyncHumiditySensorsAsync method synchronises the humidity sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncHumiditySensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the humidity sensors present in the REST humidity sensors list
                RESTHumiditySensors = await _humidityRESTService.RefreshHumiditySensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalHumiditySensors = await GetHumiditySensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var humiditySensor in LocalHumiditySensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTHumiditySensors.Any(l => l.Id == humiditySensor.Id))
                    {
                        //If the humidity sensor is not present calls the HumiditySensorRESTServices SaveHumiditySensorAsync method which will post the humidity sensor to the REST API
                        await _humidityRESTService.SaveHumiditySensorAsync(humiditySensor, true);
                    }
                }
                //Iterates through the RESTHumiditySensors list
                foreach (var humiditySensor in RESTHumiditySensors)
                {
                    //Checks if any humidity sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalHumiditySensors.Any(l => l.Id == humiditySensor.Id))
                    {
                        //If the humidity sensor is not present it is inserted into the local SQLite database
                        await AddHumiditySensorAsync(humiditySensor.Name, humiditySensor.Description, humiditySensor.BatteryLife, humiditySensor.Temperature, humiditySensor.Humidity, humiditySensor.DeviceLocationId);
                    }
                    //Checks if a humidity sensor present in both the local SQLite database and REST API but has been turned off, or has a different humidity level in the REST API
                    if (LocalHumiditySensors.Any(l => l.Id == humiditySensor.Id && (l.PowerStatus != humiditySensor.PowerStatus || l.Name != humiditySensor.Name|| l.Description != humiditySensor.Description 
                                                || l.BatteryLife != humiditySensor.BatteryLife || l.Temperature != humiditySensor.Temperature || l.DeviceLocationId != humiditySensor.DeviceLocationId)))
                    {
                        //If the humidity sensor is turned off in the REST API in memory database turn the camera off in the local SQLite database
                        await UpdateHumiditySensorAsync(humiditySensor);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync humidity sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteHumiditySensorAsync method deletes a specified humidity sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="humiditySensor"></param>
        public async Task DeleteHumiditySensorAsync(HumiditySensor humiditySensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified humidity sensor
                await _connection.DeleteAsync(humiditySensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the humidity sensor from REST API
                await _humidityRESTService.DeleteHumiditySensorAsync((int)humiditySensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete humidity sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateHumiditySensor method updates the details of the humidity sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="humiditySensor"></param>
        public async Task UpdateHumiditySensorAsync(HumiditySensor humiditySensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Sets the string property OnOrOff based on the boolean property PowerStatus for display purposes in the location page
                if (humiditySensor.PowerStatus)
                    humiditySensor.OnOrOff = "On";
                else
                    humiditySensor.OnOrOff = "Off";

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(humiditySensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await _humidityRESTService.SaveHumiditySensorAsync(humiditySensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update humidity sensor. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
