using CommunityToolkit.Mvvm.Input;
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
        //Method to sync devices present on the SQLite database with the REST API
        public async Task SyncLocationsAsync()
        {
            try
            {
                //Intialises the database if it isn't already
                await Init();
                //Refreshes the devices present in the Locations list
                Locations = await _locationRESTService.RefreshLocationsAsync();
                //Refreshes the devices present in the LocalLocations list
                LocalLocations = await GetLocationsAsync();
                //Iterates through the LocalLocation list
                foreach(var localLocation in LocalLocations)
                {
                    //Checks if any location present in the SQLite database is present in the REST API in memory database
                    if(!Locations.Any(l => l.Id == localLocation.Id && l.LocationName == localLocation.LocationName))
                    {
                        //If the device is not present calls the LocationRESTServices SaveLocationAsync method which will post the location to the REST API
                        await _locationRESTService.SaveLocationAsync(localLocation, true);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Unable to sync locations. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
