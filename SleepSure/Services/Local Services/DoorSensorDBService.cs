using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class DoorSensorDBService : IDoorSensorDataServer
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for door sensors
        IDoorSensorRESTService doorRESTService;

        //List of door sensors retrieved from the REST API
        public List<DoorSensor> RESTDoorSensors { get; private set; } = [];
        //List of door sensors retrieved from a local JSON file
        public List<DoorSensor> JSONDoorSensors { get; private set; } = [];
        //List of door sensors in the local SQLite database
        public List<DoorSensor> LocalDoorSensors { get; private set; } = [];

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

        public DoorSensorDBService(string dbPath, IDoorSensorRESTService doorSensorRESTService)
        {
            _dbPath = dbPath;
            _internet = Connectivity.Current.NetworkAccess;
            doorRESTService = doorSensorRESTService;
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
            //Creates the door sensor table
            var result = await _connection.CreateTableAsync<DoorSensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<DoorSensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default door sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default door sensors from a local JSON file
                await GetJSONDoorSensorsAsync();
                //Iterates through the list of default cameras
                foreach (var doorSensor in JSONDoorSensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(doorSensor);
                }
            }
        }
        /// <summary>
        /// The AddDoorSensor method adds a new door sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddDoorSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new door sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new DoorSensor(name, description, batteryLife, temperature, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the DoorSensor created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncDoorSensorsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add door sensor. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetDoorSensorsAsync method retrieves a list of all door sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of door sensors</returns>
        public async Task<List<DoorSensor>> GetDoorSensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all door sensors in the SQLite database
                return await _connection.Table<DoorSensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get door sensors from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of door sensors if an exception occurs
            return new List<DoorSensor>();
        }
        /// <summary>
        /// The GetJSONDoorSensorsAsync method deserialises a list of default door sensors into the JSONDoorSensors list
        /// </summary>
        public async Task GetJSONDoorSensorsAsync()
        {
            if (JSONDoorSensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoDoorSensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONDoorSensors = JsonSerializer.Deserialize<List<DoorSensor>>(content);
        }

        /// <summary>
        /// The SyncDoorSensorsAsync method synchronises the door sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncDoorSensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the door sensors present in the REST door sensors list
                RESTDoorSensors = await doorRESTService.RefreshDoorSensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalDoorSensors = await GetDoorSensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var doorSensor in LocalDoorSensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTDoorSensors.Any(l => l.Id == doorSensor.Id))
                    {
                        //If the door sensor is not present calls the DoorSensorRESTServices SaveDoorSensorAsync method which will post the door sensor to the REST API
                        await doorRESTService.SaveDoorSensorAsync(doorSensor, true);
                    }
                }
                //Iterates through the RESTDoorSensors list
                foreach (var doorSensor in RESTDoorSensors)
                {
                    //Checks if any door sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalDoorSensors.Any(l => l.Id == doorSensor.Id))
                    {
                        //If the door sensor is not present it is inserted into the local SQLite database
                        await AddDoorSensorAsync(doorSensor.Name, doorSensor.Description, doorSensor.BatteryLife, doorSensor.Temperature, doorSensor.DeviceLocationId);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync door sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteDoorSensorAsync method deletes a specified door sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="doorSensor"></param>
        public async Task DeleteDoorSensorAsync(DoorSensor doorSensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified door sensor
                await _connection.DeleteAsync(doorSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the door sensor from REST API
                await doorRESTService.DeleteDoorSensorAsync((int)doorSensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete door sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateDoorSensor method updates the details of the door sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="doorSensor"></param>
        public async Task UpdateDoorSensorAsync(DoorSensor doorSensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(doorSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await doorRESTService.SaveDoorSensorAsync(doorSensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update door sensor. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
