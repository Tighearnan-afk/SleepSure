using SleepSure.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public class SensorDBDataService : ISensorDataService
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

        readonly string _dbPath;

        public string StatusMessage;

        public SensorDBDataService(string dbPath)
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
            var result = await _connection.CreateTableAsync<Sensor>();
        }

        public async Task<List<Sensor>> GetSensorsAsync()
        {
            try
            {
                await Init();
                return await _connection.Table<Sensor>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<Sensor>();
        }

        public async Task AddSensorAsync()
        {
            int result = 0;
            try
            {
                await Init();
                result = await _connection.InsertAsync(
                    new Sensor("Dummy Sensor", "Garden", "Dummy text",56));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add sensor to the database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }

        }
    }
}
