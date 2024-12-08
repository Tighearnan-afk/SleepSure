using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace SleepSure.ViewModel
{
    [QueryProperty("Camera", "Camera")]
    public partial class VideoArchiveViewModel : BaseViewModel
    {
        //A dataservice that retrives videos from a local SQLite database
        IVideoDataService _videoDataService;

        //Allows configuration files to be utilised by the view model
        readonly IConfiguration _appConfig;

        [ObservableProperty]
        public Camera _camera;

        //An observable collection that contains videos associated with the current camera
        public ObservableCollection<Video> Videos { get; } = [];

        //An boolean property that determines whether or not the application is in demo mode
        private bool _isInDemoMode;
        public VideoArchiveViewModel(IVideoDataService videoDataService, IConfiguration AppConfig)
        {
            _videoDataService = videoDataService;
            _appConfig = AppConfig;
            //Retrieves the app settings from the App Configuration
            Settings appSettings = _appConfig.GetRequiredSection("Settings").Get<Settings>();
            //Sets the demo mode flag
            _isInDemoMode = appSettings.DemoMode;
        }

        /// <summary>
        /// The GetVideosAsync method retrieves a list of videos from the VideoDBDataservice, filters the returned list of videos and adds the videos associated witht the current camera
        /// to the observable collection of videos
        /// </summary>

        [RelayCommand]
        public async Task GetVideosAsync()
        {
            //Ensures the application is not performing another I/O operation
            if (IsBusy)
                return;

            try
            {
                //Sets the busy flag to true
                IsBusy = true;
                //Retrieves a list of videos
                var videos = await _videoDataService.GetVideosAsync(_isInDemoMode);
                //Clears the observable collection
                Videos.Clear();

                MediaSource mediaSource;
                //Iterates through the list of videos
                foreach (var video in videos)
                {
                    //The video path must be converted to a Media Source for use with the MAUI Media element view
                    mediaSource = MediaSource.FromResource(video.VideoPath);
                    //Checks if the video is associated with the current camera and if it is adds it to the observable collection
                    if (video.CameraId == Camera.Id)
                    {
                        //Video has a MediaSource field that is ignored by SQLite but is populated here to allow the collection view to dynamically retrieve videos rather than hardcode it
                        video.MediaSource = mediaSource;
                        //Adds the video to the Videos observable collection
                        Videos.Add(video);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve videos", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// The GetVideosAsync method retrieves a list of videos from the VideoDBDataservice, filters the returned list of videos and adds the videos associated witht the current camera
        /// to the observable collection of videos
        /// </summary>

        [RelayCommand]
        public async Task DeleteVideoAsync(Video video)
        {
            //Ensures the application is not performing another I/O operation
            if (IsBusy)
                return;

            try
            {
                //Sets the busy flag to true
                IsBusy = true;

                await _videoDataService.DeleteVideoAsync(video);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to delete videos", "OK");
            }
            finally
            {
                IsBusy = false;
                await GetVideosAsync();
            }
        }
    }
}
