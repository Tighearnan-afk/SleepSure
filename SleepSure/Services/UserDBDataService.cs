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
    public class UserDBDataService : IUserDataService
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

        public UserDBDataService(string dbPath)
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
            var result = await _connection.CreateTableAsync<User>();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                await Init();
                return await _connection.Table<User>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<User>();
        }

        public async Task AddUserAsync(string email, string password)
        {
            int result = 0;
            try
            {
                await Init();
                result = await _connection.InsertAsync(
                    new User(email, password));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add user to the database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
