using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class TemperatureSensorDataService : ITemperatureSensorDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for temperature sensors
        ITemperatureSensorRESTService _temperatureSensorRESTService;

        //List of temperature sensors retrieved from the REST API
        public List<TemperatureSensor> RESTTemperatureSensors { get; private set; } = [];
        //List of temperature sensors retrieved from a local JSON file
        public List<TemperatureSensor> JSONTemperatureSensors { get; private set; } = [];
        //List of temperature sensors in the local SQLite database
        public List<TemperatureSensor> LocalTemperatureSensors { get; private set; } = [];

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

        public TemperatureSensorDataService(string dbPath, ITemperatureSensorRESTService temperatureSensorRESTService)
        {
            _dbPath = dbPath;
            _internet = Connectivity.Current.NetworkAccess;
            _temperatureSensorRESTService = temperatureSensorRESTService;
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
            //Creates the temperature sensor table
            var result = await _connection.CreateTableAsync<TemperatureSensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<TemperatureSensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default temperature sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default temperature sensors from a local JSON file
                await GetJSONTemperatureSensorsAsync();
                //Iterates through the list of default cameras
                foreach (var temperatureSensor in JSONTemperatureSensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(temperatureSensor);
                }
            }
        }
        /// <summary>
        /// The AddTemperatureSensor method adds a new temperature sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddTemperatureSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new temperature sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new TemperatureSensor(name, description, batteryLife, temperature, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the TemperatureSensor created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncTemperatureSensorsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add temperature sensor. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetTemperatureSensorsAsync method retrieves a list of all temperature sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of temperature sensors</returns>
        public async Task<List<TemperatureSensor>> GetTemperatureSensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all temperature sensors in the SQLite database
                return await _connection.Table<TemperatureSensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get temperature sensors from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of temperature sensors if an exception occurs
            return new List<TemperatureSensor>();
        }
        /// <summary>
        /// The GetJSONTemperatureSensorsAsync method deserialises a list of default temperature sensors into the JSONTemperatureSensors list
        /// </summary>
        public async Task GetJSONTemperatureSensorsAsync()
        {
            if (JSONTemperatureSensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoTemperatureSensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONTemperatureSensors = JsonSerializer.Deserialize<List<TemperatureSensor>>(content);
        }

        /// <summary>
        /// The SyncTemperatureSensorsAsync method synchronises the temperature sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncTemperatureSensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the temperature sensors present in the REST temperature sensors list
                RESTTemperatureSensors = await _temperatureSensorRESTService.RefreshTemperatureSensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalTemperatureSensors = await GetTemperatureSensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var temperatureSensor in LocalTemperatureSensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTTemperatureSensors.Any(l => l.Id == temperatureSensor.Id))
                    {
                        //If the temperature sensor is not present calls the TemperatureSensorRESTServices SaveTemperatureSensorAsync method which will post the temperature sensor to the REST API
                        await _temperatureSensorRESTService.SaveTemperatureSensorAsync(temperatureSensor, true);
                    }
                }
                //Iterates through the RESTTemperatureSensors list
                foreach (var temperatureSensor in RESTTemperatureSensors)
                {
                    //Checks if any temperature sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalTemperatureSensors.Any(l => l.Id == temperatureSensor.Id))
                    {
                        //If the temperature sensor is not present it is inserted into the local SQLite database
                        await AddTemperatureSensorAsync(temperatureSensor.Name, temperatureSensor.Description, temperatureSensor.BatteryLife, temperatureSensor.Temperature, temperatureSensor.DeviceLocationId);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync temperature sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteTemperatureSensorAsync method deletes a specified temperature sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="temperatureSensor"></param>
        public async Task DeleteTemperatureSensorAsync(TemperatureSensor temperatureSensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified temperature sensor
                await _connection.DeleteAsync(temperatureSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the temperature sensor from REST API
                await _temperatureSensorRESTService.DeleteTemperatureSensorAsync((int)temperatureSensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete temperature sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateTemperatureSensor method updates the details of the temperature sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="temperatureSensor"></param>
        public async Task UpdateTemperatureSensorAsync(TemperatureSensor temperatureSensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(temperatureSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await _temperatureSensorRESTService.SaveTemperatureSensorAsync(temperatureSensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update temperature sensor. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
