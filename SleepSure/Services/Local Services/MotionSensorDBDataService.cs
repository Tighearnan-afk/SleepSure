using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class MotionSensorDBDataService : IMotionSensorDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for motion sensors
        //IMotionSensorRESTService _motionSensorRESTService;

        //List of motion sensors retrieved from the REST API
        public List<MotionSensor> RESTMotionSensors { get; private set; } = [];
        //List of motion sensors retrieved from a local JSON file
        public List<MotionSensor> JSONMotionSensors { get; private set; } = [];
        //List of motion sensors in the local SQLite database
        public List<MotionSensor> LocalMotionSensors { get; private set; } = [];

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

        public MotionSensorDBDataService(string dbPath)
        {
            _dbPath = dbPath;
            //_motionSensorRESTService = cameraRESTService;
            _internet = Connectivity.Current.NetworkAccess;
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
            //Creates the motion sensor table
            var result = await _connection.CreateTableAsync<MotionSensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<MotionSensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default motion sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default motion sensors from a local JSON file
                await GetJSONMotionSensorsAsync();
                //Iterates through the list of default cameras
                foreach (var motionSensor in JSONMotionSensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(motionSensor);
                }
            }
        }
        /// <summary>
        /// The AddMotionSensor method adds a new motion sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddMotionSensorAsync(string name, string description, int batteryLife, string temperature, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new motion sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new MotionSensor(name, description, batteryLife, temperature, deviceLocationId));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add location. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetMotionSensorsAsync method retrieves a list of all motion sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of motion sensors</returns>
        public async Task<List<MotionSensor>> GetMotionSensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all motion sensors in the SQLite database
                return await _connection.Table<MotionSensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of motion sensors if an exception occurs
            return new List<MotionSensor>();
        }
        /// <summary>
        /// The GetJSONMotionSensorsAsync method deserialises a list of default motion sensors into the JSONMotionSensors list
        /// </summary>
        public async Task GetJSONMotionSensorsAsync()
        {
            if (JSONMotionSensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoMotionSensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONMotionSensors = JsonSerializer.Deserialize<List<MotionSensor>>(content);
        }

        /// <summary>
        /// The SyncMotionSensorsAsync method synchronises the motion sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncMotionSensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the motion sensors present in the REST motion sensors list
                //RESTMotionSensors = await _motionSensorRESTService.RefreshMotionSensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalMotionSensors = await GetMotionSensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var motionSensor in LocalMotionSensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTMotionSensors.Any(l => l.Id == motionSensor.Id))
                    {
                        //If the motion sensor is not present calls the MotionSensorRESTServices SaveMotionSensorAsync method which will post the motion sensor to the REST API
                        //await _cameraRESTService.SaveCameraAsync(localCamera, true);
                    }
                }
                //Iterates through the RESTMotionSensors list
                foreach (var motionSensor in RESTMotionSensors)
                {
                    //Checks if any motion sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalMotionSensors.Any(l => l.Id == motionSensor.Id))
                    {
                        //If the motion sensor is not present it is inserted into the local SQLite database
                        await AddMotionSensorAsync(motionSensor.Name, motionSensor.Description, motionSensor.BatteryLife, motionSensor.Temperature, motionSensor.DeviceLocationId);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync motion sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteMotionSensorAsync method deletes a specified motion sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="motionSensor"></param>
        public async Task DeleteMotionSensorAsync(MotionSensor motionSensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified motion sensor
                await _connection.DeleteAsync(motionSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the motion sensor from REST API
                //await _motionSensorRESTService.DeleteCameraAsync((int)motionSensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete motion sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateMotionSensor method updates the details of the motion sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="motionSensor"></param>
        public async Task UpdateMotionSensorAsync(MotionSensor motionSensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(motionSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                //await _motionSensorRESTService.SaveCameraAsync(motionSensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update camera. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
