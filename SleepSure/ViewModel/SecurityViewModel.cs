
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
        //A service that allows motion sensors to be retrieved from a local SQLite database
        IMotionSensorDataService _motionSensorDataService;
        //A service that allows door sensors to be retrieved from a local SQLite database
        IDoorSensorDataService _doorSensorDataService;
        //A service that allows window sensors to be retrieved from a local SQLite database
        IWindowSensorDataService _windowSensorDataService;
        //A service that allows vibration sensors to be retrieved from a local SQLite database
        IVibrationSensorDataService _vibrationSensorDataService;

        //Allows configuration files to be utilised by the view model
        readonly IConfiguration _appConfig;
        //An observable collection that stores all camera registed with the application
        public ObservableCollection<Camera> Cameras { get; } = [];

        //Lists of security devices
        public List<MotionSensor> MotionSensors;
        public List<DoorSensor> DoorSensors;
        public List<WindowSensor> WindowSensors;
        public List<VibrationSensor> VibrationSensors;

        //An boolean property that determines whether or not the application is in demo mode
        private bool _isInDemoMode;

        public SecurityViewModel(ICameraDataService cameraDataService, IConfiguration AppConfig, IMotionSensorDataService motionSensorDataService, IDoorSensorDataService doorSensorDataService,
            IWindowSensorDataService windowSensorDataService, IVibrationSensorDataService vibrationSensorDataService)
        {
            _cameraDataService = cameraDataService;

            _appConfig = AppConfig;

            Settings appSettings = _appConfig.GetRequiredSection("Settings").Get<Settings>();
            _isInDemoMode = appSettings.DemoMode;

            _motionSensorDataService = motionSensorDataService;
            _doorSensorDataService = doorSensorDataService;
            _windowSensorDataService = windowSensorDataService;
            _vibrationSensorDataService = vibrationSensorDataService;
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
        /// <summary>
        /// The GoToVideoFeedAsync method navigates to the VideoFeed page passing in a specified camera
        /// </summary>
        /// <param name="camera"></param>
        [RelayCommand]
        public async Task GoToVideoFeedAsync(Camera camera)
        {
            //Ensures the selected camera exists
            if (camera is null)
                return;

            //Check if the camera is turned on
            if (!camera.PowerStatus)
            {
                await Shell.Current.DisplayAlert("No Power", $"Please turn on the {camera.Name} camera to resume the livestream", "OK");
                return;
            }
            //Navigate to the video feed page passing the selected camera object within a dictionary and a true value for animate
            await Shell.Current.GoToAsync($"{nameof(VideoFeed)}", true,
                new Dictionary<string, object> { { "Camera", camera } });
        }

        /// <summary>
        /// The EnableSecurityDevicesAsync method retrieves a list of security devices and sets the power status to True
        /// </summary>
        [RelayCommand]
        public async Task EnableSecurityDevicesAsync()
        {
            //Ensure the application is not performing another I/O operation
            if (IsBusy)
                return;
            try
            {
                //Set the busy flag to true
                IsBusy = true;

                //Fetch a list of each security device and populate the appropriate list
                DoorSensors = await _doorSensorDataService.GetDoorSensorsAsync(_isInDemoMode);
                WindowSensors = await _windowSensorDataService.GetWindowSensorsAsync(_isInDemoMode);
                MotionSensors = await _motionSensorDataService.GetMotionSensorsAsync(_isInDemoMode);
                VibrationSensors = await _vibrationSensorDataService.GetVibrationSensorsAsync(_isInDemoMode);

                //Iterate through each list of security devices and set their power status to true
                foreach (var doorSensor in DoorSensors)
                {
                    doorSensor.PowerStatus = true;
                    await _doorSensorDataService.UpdateDoorSensorAsync(doorSensor);
                }

                foreach (var windowSensor in WindowSensors)
                {
                    windowSensor.PowerStatus = true;
                    await _windowSensorDataService.UpdateWindowSensorAsync(windowSensor);
                }

                foreach (var motionSensor in MotionSensors)
                {
                    motionSensor.PowerStatus = true;
                    await _motionSensorDataService.UpdateMotionSensorAsync(motionSensor);
                }

                foreach (var vibrationSensor in VibrationSensors)
                {
                    vibrationSensor.PowerStatus = true;
                    await _vibrationSensorDataService.UpdateVibrationSensorAsync(vibrationSensor);
                }

                foreach (var camera in Cameras)
                {
                    camera.PowerStatus = true;
                    await _cameraDataService.UpdateCameraAsync(camera);
                }

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
                //Alert the user that the operation is successful
                await Shell.Current.DisplayAlert("Security System Armed", "All security devices have been turned on and armed", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //Set the IsBusyFlag to false
                IsBusy = false;
            }
        }

        /// <summary>
        /// The DisableSecurityDevicesAsync method retrieves a list of security devices and sets the power status to False
        /// </summary>
        [RelayCommand]
        public async Task DisableSecurityDevicesAsync()
        {
            //Ensure the application is not performing another I/O operation
            if (IsBusy)
                return;
            try
            {
                //Set the busy flag to true
                IsBusy = true;

                //Fetch a list of each security device and populate the appropriate list
                DoorSensors = await _doorSensorDataService.GetDoorSensorsAsync(_isInDemoMode);
                WindowSensors = await _windowSensorDataService.GetWindowSensorsAsync(_isInDemoMode);
                MotionSensors = await _motionSensorDataService.GetMotionSensorsAsync(_isInDemoMode);
                VibrationSensors = await _vibrationSensorDataService.GetVibrationSensorsAsync(_isInDemoMode);

                //Iterate through each list of security devices and set their power status to true
                foreach (var doorSensor in DoorSensors)
                {
                    doorSensor.PowerStatus = false;
                    await _doorSensorDataService.UpdateDoorSensorAsync(doorSensor);
                }

                foreach (var windowSensor in WindowSensors)
                {
                    windowSensor.PowerStatus = false;
                    await _windowSensorDataService.UpdateWindowSensorAsync(windowSensor);
                }

                foreach (var motionSensor in MotionSensors)
                {
                    motionSensor.PowerStatus = false;
                    await _motionSensorDataService.UpdateMotionSensorAsync(motionSensor);
                }

                foreach (var vibrationSensor in VibrationSensors)
                {
                    vibrationSensor.PowerStatus = false;
                    await _vibrationSensorDataService.UpdateVibrationSensorAsync(vibrationSensor);
                }

                foreach (var camera in Cameras)
                {
                    camera.PowerStatus = false;
                    await _cameraDataService.UpdateCameraAsync(camera);
                }

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
                //Alert the user that the operation is successful
                await Shell.Current.DisplayAlert("Security System Armed", "All security devices have been turned on and armed", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //Set the IsBusyFlag to false
                IsBusy = false;
            }
        }
    }
}
