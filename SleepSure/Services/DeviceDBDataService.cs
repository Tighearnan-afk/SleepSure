using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public class DeviceDBDataService : IDeviceDataService
    {
        //SQLite connection
        SQLiteAsyncConnection _connection;

        private const SQLite.SQLiteOpenFlags _dbFLags =
            //Open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            //Create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            //Enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        string _dbPath;

        List<Model.Device> devices = new List<Model.Device>();

        public string StatusMessage;

        public DeviceDBDataService(string dbPath)
        {
            _dbPath = dbPath;
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
            var result = await _connection.CreateTableAsync<Model.Device>();
        }

        public async Task<List<Model.Device>> GetDevicesAsync()
        {
            try
            {
                await Init();
                return await _connection.Table<Model.Device>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<Model.Device>();
        }

        public async Task AddDeviceAsync()
        {
            int result = 0;
            try
            {
                await Init();
                result = await _connection.InsertAsync(
                    new Model.Device("Dummy Device", "Kitchen", "Dummy text"));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add device to the database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }

        }
    }
}
