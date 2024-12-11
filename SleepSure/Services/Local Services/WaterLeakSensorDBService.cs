using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class WaterLeakSensorDBService : IWaterLeakSensorDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for waterleak sensors
        IWaterLeakSensorRESTService waterLeakRESTService;
        //Alarm service for raising alarms
        IAlarmDataService _alarmDataService;

        //List of waterleak sensors retrieved from the REST API
        public List<WaterLeakSensor> RESTWaterLeakSensors { get; private set; } = [];
        //List of waterleak sensors retrieved from a local JSON file
        public List<WaterLeakSensor> JSONWaterLeakSensors { get; private set; } = [];
        //List of waterleak sensors in the local SQLite database
        public List<WaterLeakSensor> LocalWaterLeakSensors { get; private set; } = [];

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

        public WaterLeakSensorDBService(string dbPath, IWaterLeakSensorRESTService waterLeakSensorRESTService, IAlarmDataService alarmDataService)
        {
            _dbPath = dbPath;
            //waterLeakRESTService = cameraRESTService;
            _internet = Connectivity.Current.NetworkAccess;
            waterLeakRESTService = waterLeakSensorRESTService;
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
            //Creates the waterleak sensor table
            var result = await _connection.CreateTableAsync<WaterLeakSensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<WaterLeakSensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default waterleak sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default waterleak sensors from a local JSON file
                await GetJSONWaterLeakSensorsAsync();
                //Iterates through the list of default cameras
                foreach (var waterLeakSensor in JSONWaterLeakSensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(waterLeakSensor);
                }
            }
        }
        /// <summary>
        /// The AddWaterLeakSensor method adds a new waterleak sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddWaterLeakSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new waterleak sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new WaterLeakSensor(name, description, batteryLife, temperature, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the WaterLeakSensor created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncWaterLeakSensorsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add waterleak sensor. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetWaterLeakSensorsAsync method retrieves a list of all waterleak sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of waterleak sensors</returns>
        public async Task<List<WaterLeakSensor>> GetWaterLeakSensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all waterleak sensors in the SQLite database
                return await _connection.Table<WaterLeakSensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get waterleak sensors from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of waterleak sensors if an exception occurs
            return new List<WaterLeakSensor>();
        }
        /// <summary>
        /// The GetJSONWaterLeakSensorsAsync method deserialises a list of default waterleak sensors into the JSONWaterLeakSensors list
        /// </summary>
        public async Task GetJSONWaterLeakSensorsAsync()
        {
            if (JSONWaterLeakSensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoWaterLeakSensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONWaterLeakSensors = JsonSerializer.Deserialize<List<WaterLeakSensor>>(content);
        }

        /// <summary>
        /// The SyncWaterLeakSensorsAsync method synchronises the waterleak sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncWaterLeakSensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the waterleak sensors present in the REST waterleak sensors list
                RESTWaterLeakSensors = await waterLeakRESTService.RefreshWaterLeakSensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalWaterLeakSensors = await GetWaterLeakSensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var waterLeakSensor in LocalWaterLeakSensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTWaterLeakSensors.Any(l => l.Id == waterLeakSensor.Id))
                    {
                        //If the waterleak sensor is not present calls the WaterLeakSensorRESTServices SaveWaterLeakSensorAsync method which will post the waterleak sensor to the REST API
                        await waterLeakRESTService.SaveWaterLeakSensorAsync(waterLeakSensor, true);
                    }
                }
                //Iterates through the RESTWaterLeakSensors list
                foreach (var waterLeakSensor in RESTWaterLeakSensors)
                {
                    //Checks if any waterleak sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalWaterLeakSensors.Any(l => l.Id == waterLeakSensor.Id))
                    {
                        //If the waterleak sensor is not present it is inserted into the local SQLite database
                        await AddWaterLeakSensorAsync(waterLeakSensor.Name, waterLeakSensor.Description, waterLeakSensor.BatteryLife, waterLeakSensor.Temperature, waterLeakSensor.DeviceLocationId);
                    }
                    //Checks if a waterleak sensor is present in both the local SQLite database and REST API but has different details in the REST API
                    if (LocalWaterLeakSensors.Any(l => l.Id == waterLeakSensor.Id && (l.PowerStatus != waterLeakSensor.PowerStatus || l.LeakDetected != waterLeakSensor.LeakDetected || l.Name != waterLeakSensor.Name
                        || l.Description != waterLeakSensor.Description || l.BatteryLife != waterLeakSensor.BatteryLife || l.Temperature != waterLeakSensor.Temperature || l.DeviceLocationId != waterLeakSensor.DeviceLocationId)))
                    {
                        //If the waterleak sensor detects an open window raise the alarm
                        if (waterLeakSensor.LeakDetected && waterLeakSensor.PowerStatus)
                            await _alarmDataService.CreateAlarmAsync("Waterleak Detected", $"Waterleak Detected detected by {waterLeakSensor.Name}", waterLeakSensor.Name, DateTime.Now);
                        //If the waterleak sensor is turned off in the REST API in memory database turn the camera off in the local SQLite database
                        await UpdateWaterLeakSensorAsync(waterLeakSensor);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync waterleak sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteWaterLeakSensorAsync method deletes a specified waterleak sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="waterLeakSensor"></param>
        public async Task DeleteWaterLeakSensorAsync(WaterLeakSensor waterLeakSensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified waterleak sensor
                await _connection.DeleteAsync(waterLeakSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the waterleak sensor from REST API
                await waterLeakRESTService.DeleteWaterLeakSensorAsync((int)waterLeakSensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete waterleak sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateWaterLeakSensor method updates the details of the waterleak sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="waterLeakSensor"></param>
        public async Task UpdateWaterLeakSensorAsync(WaterLeakSensor waterLeakSensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Sets the string property OnOrOff based on the boolean property PowerStatus for display purposes in the location page
                if (waterLeakSensor.PowerStatus)
                    waterLeakSensor.OnOrOff = "On";
                else
                    waterLeakSensor.OnOrOff = "Off";

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(waterLeakSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await waterLeakRESTService.SaveWaterLeakSensorAsync(waterLeakSensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update waterleak sensor. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
