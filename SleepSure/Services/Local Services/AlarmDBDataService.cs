using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class AlarmDBDataService : IAlarmDataService
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

        public AlarmDBDataService(string dbPath)
        {
            _dbPath = dbPath;
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
            //Creates the Alarm table
            var result = await _connection.CreateTableAsync<Alarm>();
        }

        public async Task CreateAlarmAsync(string eventName, string eventDescription, string deviceName, DateTime dateTime)
        {
            int result = 0;
            try
            {
                await Init();

                await _connection.InsertAsync(new Alarm(eventName, eventDescription, deviceName, dateTime));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add alarm. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        public async Task<List<Alarm>> GetAlarmsAsync()
        {
            try
            {
                await Init();
                return await _connection.Table<Alarm>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<Alarm>();
        }

        /// <summary>
        /// The DeleteAlarmAsync method deletes a specified Alarm from the local SQLite database
        /// </summary>
        /// <param name="Alarm"></param>
        public async Task RemoveAlarmAsync(Alarm Alarm)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();

                //Delete the specified Alarm
                await _connection.DeleteAsync(Alarm);

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete alarm from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
