using SleepSure.Model;
using SQLite;
using System.Diagnostics;

namespace SleepSure.Services
{
    public class CameraDBDataService : ICameraDataService
    {
        //REST API service for cameras
        ICameraRESTService _cameraRESTService;
        //List of cameras retrieved from the REST API
        public List<Camera> Cameras { get; private set; } = [];
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

        public CameraDBDataService(string dbPath, ICameraRESTService cameraRESTService)
        {
            _dbPath = dbPath;
            _cameraRESTService = cameraRESTService;
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
            var result = await _connection.CreateTableAsync<Camera>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<Camera>().CountAsync();
            //If no rows exist seeds the SQLite database with data fetched from the REST API
            if(tableData == 0)
            {
                Cameras = await _cameraRESTService.RefreshCamerasAsync();
                foreach(var camera in Cameras)
                {
                    await _connection.InsertAsync(camera);
                }
            }
        }


        public async Task AddCameraAsync()
        {
            await Init();
            return;
        }

        public async Task<List<Camera>> GetCamerasAsync()
        {
            try
            {
                await Init();
                return await _connection.Table<Camera>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get devices from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<Camera>();
        }
    }
}
