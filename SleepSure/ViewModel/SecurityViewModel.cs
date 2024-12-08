

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using SleepSure.Model;
using SleepSure.Pages;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    public partial class SecurityViewModel : BaseViewModel
    {
        //A service that allows cameras to be retrieved from a local SQLite database
        ICameraDataService _cameraDataService;
        //Allows configuration files to be utilised by the view model
        readonly IConfiguration _appConfig;
        //An observable collection that stores all camera registed with the application
        public ObservableCollection<Camera> Cameras { get; } = [];

        //An boolean property that determines whether or not the application is in demo mode
        private bool _isInDemoMode;

        public SecurityViewModel(ICameraDataService cameraDataService, IConfiguration AppConfig) 
        { 
            _cameraDataService = cameraDataService;

            _appConfig = AppConfig;

            Settings appSettings = _appConfig.GetRequiredSection("Settings").Get<Settings>();
            _isInDemoMode = appSettings.DemoMode;
        }

        /// <summary>
        /// The GetCamerasAsync method retrieves a list of cameras registerd with the application and inserts them into the Cameras observable collection
        /// </summary>

        [RelayCommand]
        public async Task GetCamerasAsync()
        {
            //Ensures the application is not busy performing another I/O operation
            if (IsBusy)
                return;

            try
            {
                //Sets the IsBusy property to be true
                IsBusy = true;
                //Retrieves a list of cameras from the CameraDBDataservice
                var cameras = await _cameraDataService.GetCamerasAsync(_isInDemoMode);
                //Refreshes the Cameras observable collection
                Cameras.Clear();
                //Iterates through the cameras list retrieved from the local SQLite database
                foreach (var camera in cameras)
                {
                    //Adds the camera to the Cameras observable collection
                    Cameras.Add(camera);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve cameras", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToVideoFeedAsync(Camera camera)
        {
            //Ensures the selected camera exists
            if (camera is null)
                return;
            //Navigate to the video feed page passing the selected camera object within a dictionary and a true value for animate
            await Shell.Current.GoToAsync($"{nameof(VideoFeed)}", true,
                new Dictionary<string, object> { { "Camera", camera } });
        }

        [RelayCommand]
        public async Task ToggleCameraAsync(Camera camera)
        {
            await _cameraDataService.UpdateCameraAsync(camera);
        }
    }
}
