using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{ 
    public class LightDBDataService : ILightDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;
        //REST API service for lights
        ILightRESTService _lightRESTService;

        //List of lights retrieved from the REST API
        public List<Light> RESTLights { get; private set; } = [];
        //List of lights retrieved from a local JSON file
        public List<Light> JSONLights { get; private set; } = [];
        //List of lights in the local SQLite database
        public List<Light> LocalLights { get; private set; } = [];

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

        public LightDBDataService(string dbPath, ILightRESTService lightRESTService)
        {
            _dbPath = dbPath;
            _internet = Connectivity.Current.NetworkAccess;
            _lightRESTService = lightRESTService;
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
            //Creates the light table
            var result = await _connection.CreateTableAsync<Light>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<Light>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default lights stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default lights from a local JSON file
                await GetJSONLightsAsync();
                //Iterates through the list of default cameras
                foreach (var light in JSONLights)
                {
                    //Inserts the default camera into the SQLite database
                    await _connection.InsertAsync(light);
                }
            }
        }
        /// <summary>
        /// The AddLight method adds a new light to the local SQLite database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="deviceLocationId"></param>
        public async Task AddLightAsync(string name, string description, int brightness, int deviceLocationId)
        {
            int result = 0;
            try
            {
                //Ensure the connection to the database is created
                await Init();
                //Insert the new light into the SQLite database with the provided parameters
                result = await _connection.InsertAsync(new Light(name, description, brightness, deviceLocationId));

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Call the sync method as the Light created is assigned an Id by SQLite and therefore cannot be used with the REST save method
                await SyncLightsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add location. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
        /// <summary>
        /// The GetLightsAsync method retrieves a list of all lights from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of lights</returns>
        public async Task<List<Light>> GetLightsAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the IsInDemoMode flag
                _isInDemoMode = isInDemoMode;
                //Ensure the connection to the database is created
                await Init();
                //Return a list of all lights in the SQLite database
                return await _connection.Table<Light>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            //Return a list of empty list of lights if an exception occurs
            return new List<Light>();
        }
        /// <summary>
        /// The GetJSONLightsAsync method deserialises a list of default lights into the JSONLights list
        /// </summary>
        public async Task GetJSONLightsAsync()
        {
            if (JSONLights.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoLights.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONLights = JsonSerializer.Deserialize<List<Light>>(content);
        }

        /// <summary>
        /// The SyncLightsAsync method synchronises the lights between the local SQLite database and the REST API
        /// </summary>

        public async Task SyncLightsAsync()
        {
            //Checks if the device has an internet connection
            if (_internet != NetworkAccess.Internet)
                return;

            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the lights present in the REST lights list
                RESTLights = await _lightRESTService.RefreshLightsAsync();
                //Refreshes the cameras present in the local cameras list
                LocalLights = await GetLightsAsync(_isInDemoMode);
                //Iterates through the LocalCameras list
                foreach (var light in LocalLights)
                {
                    //Checks if any camera present in the SQLite database is present in the REST API in memory database
                    if (!RESTLights.Any(l => l.Id == light.Id))
                    {
                        //If the light is not present calls the LightRESTServices SaveLightAsync method which will post the light to the REST API
                        await _lightRESTService.SaveLightAsync(light, true);
                    }
                }
                //Iterates through the RESTLights list
                foreach (var light in RESTLights)
                {
                    //Checks if any light present in the REST API in memory database is not present in the SQLite database
                    if (!LocalLights.Any(l => l.Id == light.Id))
                    {
                        //If the light is not present it is inserted into the local SQLite database
                        await AddLightAsync(light.Name, light.Description, light.Brightness, light.DeviceLocationId);
                    }
                    //Checks if a light is present in both the local SQLite database and REST API but has different details
                    if (LocalLights.Any(l => l.Id == light.Id && (l.PowerStatus != light.PowerStatus || l.Name != light.Name || l.Brightness != light.Brightness || l.Description != light.Description 
                                        || l.DeviceLocationId != light.DeviceLocationId)))
                    {
                        //If the light is turned off in the REST API in memory database turn the camera off in the local SQLite database
                        await UpdateLightAsync(light);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync lights. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The DeleteLightAsync method deletes a specified light from the local SQLite database and the REST API
        /// </summary>
        /// <param name="light"></param>
        public async Task DeleteLightAsync(Light light)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified light
                await _connection.DeleteAsync(light);

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Delete the light from REST API
                await _lightRESTService.DeleteLightAsync((int)light.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete light from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The UpdateLight method updates the details of the light in the local SQLite database and the REST API
        /// </summary>
        /// <param name="light"></param>
        public async Task UpdateLightAsync(Light light)
        {
            //Records the result of the SQLite operation
            int result = 0;
            try
            {
                //Ensures the connection to the SQLite database is created
                await Init();

                //Updates the camera details and records the result
                result = await _connection.UpdateAsync(light);

                //Sets the string property OnOrOff based on the boolean property PowerStatus for display purposes in the location page
                if (light.PowerStatus)
                    light.OnOrOff = "On";
                else
                    light.OnOrOff = "Off";

                //Checks if the device has an internet connection
                if (_internet != NetworkAccess.Internet)
                    return;

                //Update the camera in the REST API by calling the SaveCameraAsync method and providing a isNewCamera value of false
                await _lightRESTService.SaveLightAsync(light, false);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to update light. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
