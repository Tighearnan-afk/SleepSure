using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class VibrationSensorDBService : IVibrationSensorDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        readonly NetworkAccess _internet;
        //REST API service for vibration sensors
        readonly IVibrationSensorRESTService _vibrationRESTService;
        //Alarm service for raising alarms
        readonly IAlarmDataService _alarmDataService;

        //List of vibration sensors retrieved from the REST API
        public List<VibrationSensor> RESTVibrationSensors { get; private set; } = [];
        //List of vibration sensors retrieved from a local JSON file
        public List<VibrationSensor> JSONVibrationSensors { get; private set; } = [];
        //List of vibration sensors in the local SQLite database
        public List<VibrationSensor> LocalVibrationSensors { get; private set; } = [];

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

        public VibrationSensorDBService(string dbPath, IVibrationSensorRESTService vibrationSensorRESTService, IAlarmDataService alarmDataService)
        {
            _dbPath = dbPath;
            _internet = Connectivity.Current.NetworkAccess;
            _vibrationRESTService = vibrationSensorRESTService;
            _alarmDataService = alarmDataService;
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
            //Creates the vibration sensor table
            var result = await _connection.CreateTableAsync<VibrationSensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<VibrationSensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default vibration sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default vibration sensors from a local JSON file
                await GetJSONVibrationSensorsAsync();
                //Iterates through the list of default cameras
                foreach (var vibrationSensor in JSONVibrationSensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(vibrationSensor);
                }
            }
        }
        /// <summary>
        /// The AddVibrationSensor method adds a new vibration sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddVibrationSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new vibration sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new VibrationSensor(name, description, batteryLife, temperature, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the VibrationSensor created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncVibrationSensorsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add vibration sensor. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetVibrationSensorsAsync method retrieves a list of all vibration sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of vibration sensors</returns>
        public async Task<List<VibrationSensor>> GetVibrationSensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all vibration sensors in the SQLite database
                return await _connection.Table<VibrationSensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get vibration sensors from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of vibration sensors if an exception occurs
            return new List<VibrationSensor>();
        }
        /// <summary>
        /// The GetJSONVibrationSensorsAsync method deserialises a list of default vibration sensors into the JSONVibrationSensors list
        /// </summary>
        public async Task GetJSONVibrationSensorsAsync()
        {
            if (JSONVibrationSensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoVibrationSensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONVibrationSensors = JsonSerializer.Deserialize<List<VibrationSensor>>(content);
        }

        /// <summary>
        /// The SyncVibrationSensorsAsync method synchronises the vibration sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncVibrationSensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the vibration sensors present in the REST vibration sensors list
                RESTVibrationSensors = await _vibrationRESTService.RefreshVibrationSensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalVibrationSensors = await GetVibrationSensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var vibrationSensor in LocalVibrationSensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTVibrationSensors.Any(l => l.Id == vibrationSensor.Id))
                    {
                        //If the vibration sensor is not present calls the VibrationSensorRESTServices SaveVibrationSensorAsync method which will post the vibration sensor to the REST API
                        await _vibrationRESTService.SaveVibrationSensorAsync(vibrationSensor, true);
                    }
                }
                //Iterates through the RESTVibrationSensors list
                foreach (var vibrationSensor in RESTVibrationSensors)
                {
                    //Checks if any vibration sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalVibrationSensors.Any(l => l.Id == vibrationSensor.Id))
                    {
                        //If the vibration sensor is not present it is inserted into the local SQLite database
                        await AddVibrationSensorAsync(vibrationSensor.Name, vibrationSensor.Description, vibrationSensor.BatteryLife, vibrationSensor.Temperature, vibrationSensor.DeviceLocationId);
                    }
                    //Checks if a vibration sensor is present in both the local SQLite database and REST API but has different details in the REST API
                    if (LocalVibrationSensors.Any(l => l.Id == vibrationSensor.Id && (l.PowerStatus != vibrationSensor.PowerStatus || l.VibrationDetected != vibrationSensor.VibrationDetected || l.Name != vibrationSensor.Name
                        || l.Description != vibrationSensor.Description || l.BatteryLife != vibrationSensor.BatteryLife || l.Temperature != vibrationSensor.Temperature || l.DeviceLocationId != vibrationSensor.DeviceLocationId)))
                    {
                        //If the window sensor detects an open window raise the alarm
                        if (vibrationSensor.VibrationDetected && vibrationSensor.PowerStatus)
                            await _alarmDataService.CreateAlarmAsync("Vibration Detected", $"Vibration detected by {vibrationSensor.Name}", vibrationSensor.Name, DateTime.Now);
                        //If the vibration sensor is turned off in the REST API in memory database turn the camera off in the local SQLite database
                        await UpdateVibrationSensorAsync(vibrationSensor);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync vibration sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteVibrationSensorAsync method deletes a specified vibration sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="vibrationSensor"></param>
        public async Task DeleteVibrationSensorAsync(VibrationSensor vibrationSensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified vibration sensor
                await _connection.DeleteAsync(vibrationSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the vibration sensor from REST API
                await _vibrationRESTService.DeleteVibrationSensorAsync((int)vibrationSensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete vibration sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateVibrationSensor method updates the details of the vibration sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="vibrationSensor"></param>
        public async Task UpdateVibrationSensorAsync(VibrationSensor vibrationSensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Sets the string property OnOrOff based on the boolean property PowerStatus for display purposes in the location page
                if (vibrationSensor.PowerStatus)
                    vibrationSensor.OnOrOff = "On";
                else
                    vibrationSensor.OnOrOff = "Off";

                //Updates the vibration sensor details and records the result
                result = await _connection.UpdateAsync(vibrationSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await _vibrationRESTService.SaveVibrationSensorAsync(vibrationSensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update vibration sensor. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
