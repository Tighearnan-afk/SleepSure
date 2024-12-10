using SleepSure.Model;
using SQLite;
using System.Diagnostics;
using System.Text.Json;

namespace SleepSure.Services
{
    public class VideoDBDataService : IVideoDataService
    {
        //Network access variable that will be used to determine if the device has an internet connection
        NetworkAccess _internet;

        //List of videos retrieved from a local JSON file
        public List<Video> JSONVideos { get; private set; } = [];
        //List of videos in the local SQLite database
        public List<Video> LocalVideos { get; private set; } = [];
        //SQLite connection
        SQLiteAsyncConnection _connection;

        private const SQLite.SQLiteOpenFlags _dbFLags =
            //Open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            //Create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            //Enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        //Contains a string that contains the path to the database on the device
        readonly string _dbPath;

        public string StatusMessage;

        private bool _isInDemoMode;
        public VideoDBDataService(string dbPath)
        {
            _dbPath = dbPath;
            _internet = Connectivity.Current.NetworkAccess;
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
            //Creates the video table
            var result = await _connection.CreateTableAsync<Video>();
            //Checks if any rows exist in the database
            var tableData = await _connection.Table<Video>().CountAsync();
            //If no rows exist and the application is in demo mode seed the database with a list of default locations stored in a local json file
            if (tableData == 0 && _isInDemoMode)
            {
                //Fetches a list of default videos from a local JSON file
                await GetJSONVideosAsync();
                //Iterates through the list of default videos
                foreach (var video in JSONVideos)
                {
                    //Inserts the default location into the SQLite database
                    await _connection.InsertAsync(video);
                }
            }
        }

        /// <summary>
        /// The AddVideoAsync method inserts a new video into the local SQLite database
        /// </summary>
        /// <param name="cameraId"></param>

        public async Task AddVideoAsync(int cameraId)
        {
            int result = 0;
            try
            {
                await Init();
                result = await _connection.InsertAsync(new Video(cameraId));
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add video. Error: {0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }

        /// <summary>
        /// The GetVideosAsync method returns a list of videos retrieved from the local SQLite database
        /// </summary>
        /// <param name="isInDemoMode"></param>
        /// <returns>A list of videos</returns>
        public async Task<List<Video>> GetVideosAsync(bool isInDemoMode)
        {
            try
            {
                //Sets the demo mode flag based on if the application is in demo mode or not
                _isInDemoMode = isInDemoMode;
                //Ensures the connection to the database is initialised
                await Init();
                //Returns a list locations retrieved from the local SQLite database
                return await _connection.Table<Video>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to get videos from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
            return new List<Video>();
        }
        /// <summary>
        /// The GetJSONVideosAsync method deserialises a list of default videos into the JSONVideos list
        /// </summary>
        public async Task GetJSONVideosAsync()
        {
            //Ensures the JSON videos list is empty
            if (JSONVideos.Count > 0)
                return;

            //Load JSON data from file
            using var stream = await FileSystem.OpenAppPackageFileAsync("Demo/DemoVideos.json");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            //Deserialises the contents of the demo locations JSON file into the JSON locations list
            JSONVideos = JsonSerializer.Deserialize<List<Video>>(content);
        }

        /// <summary>
        /// The DeleteVideoAsync method deletes a specified video from the local SQLite database
        /// </summary>
        /// <param name="video"></param>
        public async Task DeleteVideoAsync(Video video)
        {
            try
            {
                //Ensure a connection to the database is created
                await Init();
                //Delete the specified video
                await _connection.DeleteAsync(video);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete video from database. Error{0}", ex.Message);
                Debug.WriteLine(StatusMessage);
            }
        }
    }
}
