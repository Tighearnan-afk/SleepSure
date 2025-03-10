﻿using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class UserDBDataService : IUserDataService
    {
        //List of users retrieved from the SQLite database
        public List<User> Users { get; private set; } = [];
        //List of users retrieved from a local JSON file
        public List<User> JSONUsers { get; private set; } = [];
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
        public UserDBDataService(string dbPath)
        {
            _dbPath = dbPath;
        }

        /// <summary>
        /// Initialise the connection to the database
        /// </summary>

        public async Task Init()
        {
            //If the database has already been created this method does nothing
            if (_connection is not null)
                return;
            //Creates a connection using the database path and its flags
            _connection = new SQLiteAsyncConnection(_dbPath, _dbFLags);
            //Creates the device table
            var result = await _connection.CreateTableAsync<User>();

            //Checks if any rows exist in the database
            var tableData = await _connection.Table<User>().CountAsync();

            if (tableData == 0 && _isInDemoMode)
            {
                await GetJSONUsersAsync();

                foreach (var user in JSONUsers)
                {
                    await _connection.InsertAsync(user);
                }
            }
        }

        public async Task<List<User>> GetUsersAsync(bool isInDemoMode)
        {
            try
            {
                _isInDemoMode = isInDemoMode;
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

        public async Task GetJSONUsersAsync()
        {
            if (JSONUsers.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoUsers.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            JSONUsers = JsonSerializer.Deserialize<List<User>>(content);
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
