using SleepSure.Model;
using SleepSure.Services.REST_Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services.DB_Services
{
    public class DeviceLocationDBService : IDeviceLocationDataService
    {
        //REST API service for cameras
        IDeviceLocationRESTService _locationRESTService;
        //List of cameras retrieved from the REST API
        public List<DeviceLocation> Locations { get; private set; } = [];
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

        public DeviceLocationDBService(string dbPath, IDeviceLocationRESTService locationRESTService)
        {
            _dbPath = dbPath;
            _locationRESTService = locationRESTService;
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
            //If no rows exist seeds the SQLite database with data fetched from the REST API
            if (tableData == 0)
            {
                Locations = await _locationRESTService.RefreshLocationsAsync();
                foreach (var locations in Locations)
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

                await _locationRESTService.SaveLocationAsync(new DeviceLocation(locationName), true);

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add location. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        public async Task<List<DeviceLocation>> GetLocationsAsync()
        {
            try
            {
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
    }
}
