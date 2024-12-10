using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class WindowSensorDBDataService : IWindowSensorDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for window sensors
        IWindowSensorRESTService windowRESTService;

        //List of window sensors retrieved from the REST API
        public List<WindowSensor> RESTWindowSensors { get; private set; } = [];
        //List of window sensors retrieved from a local JSON file
        public List<WindowSensor> JSONWindowSensors { get; private set; } = [];
        //List of window sensors in the local SQLite database
        public List<WindowSensor> LocalWindowSensors { get; private set; } = [];

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

        public WindowSensorDBDataService(string dbPath, IWindowSensorRESTService windowSensorRESTService)
        {
            _dbPath = dbPath;
            _internet = Connectivity.Current.NetworkAccess;
            windowRESTService = windowSensorRESTService;
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
            //Creates the window sensor table
            var result = await _connection.CreateTableAsync<WindowSensor>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<WindowSensor>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default window sensors stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default window sensors from a local JSON file
                await GetJSONWindowSensorsAsync();
                //Iterates through the list of default cameras
                foreach (var windowSensor in JSONWindowSensors)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(windowSensor);
                }
            }
        }
        /// <summary>
        /// The AddWindowSensor method adds a new window sensor to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="batteryLife"></param>
        /// <param name="temperature"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddWindowSensorAsync(string name, string description, int batteryLife, int temperature, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new window sensor into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new WindowSensor(name, description, batteryLife, temperature, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the WindowSensor created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncWindowSensorsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add window sensor. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetWindowSensorsAsync method retrieves a list of all window sensors from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of window sensors</returns>
        public async Task<List<WindowSensor>> GetWindowSensorsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all window sensors in the SQLite database
                return await _connection.Table<WindowSensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get window sensors from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of window sensors if an exception occurs
            return new List<WindowSensor>();
        }
        /// <summary>
        /// The GetJSONWindowSensorsAsync method deserialises a list of default window sensors into the JSONWindowSensors list
        /// </summary>
        public async Task GetJSONWindowSensorsAsync()
        {
            if (JSONWindowSensors.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoWindowSensors.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONWindowSensors = JsonSerializer.Deserialize<List<WindowSensor>>(content);
        }

        /// <summary>
        /// The SyncWindowSensorsAsync method synchronises the window sensors between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncWindowSensorsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the window sensors present in the REST window sensors list
                RESTWindowSensors = await windowRESTService.RefreshWindowSensorsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalWindowSensors = await GetWindowSensorsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var windowSensor in LocalWindowSensors)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTWindowSensors.Any(l => l.Id == windowSensor.Id))
                    {
                        //If the window sensor is not present calls the WindowSensorRESTServices SaveWindowSensorAsync method which will post the window sensor to the REST API
                        await windowRESTService.SaveWindowSensorAsync(windowSensor, true);
                    }
                }
                //Iterates through the RESTWindowSensors list
                foreach (var windowSensor in RESTWindowSensors)
                {
                    //Checks if any window sensor present in the REST API in memory database is not present in the SQLite database
                    if (!LocalWindowSensors.Any(l => l.Id == windowSensor.Id))
                    {
                        //If the window sensor is not present it is inserted into the local SQLite database
                        await AddWindowSensorAsync(windowSensor.Name, windowSensor.Description, windowSensor.BatteryLife, windowSensor.Temperature, windowSensor.DeviceLocationId);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync window sensors. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteWindowSensorAsync method deletes a specified window sensor from the local SQLite database and the REST API
        /// </summary>
        /// <param name="windowSensor"></param>
        public async Task DeleteWindowSensorAsync(WindowSensor windowSensor)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified window sensor
                await _connection.DeleteAsync(windowSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the window sensor from REST API
                await windowRESTService.DeleteWindowSensorAsync((int)windowSensor.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete window sensor from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateWindowSensor method updates the details of the window sensor in the local SQLite database and the REST API
        /// </summary>
        /// <param name="windowSensor"></param>
        public async Task UpdateWindowSensorAsync(WindowSensor windowSensor)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(windowSensor);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await windowRESTService.SaveWindowSensorAsync(windowSensor, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update window sensor. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
