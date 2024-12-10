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
    [QueryProperty("Location","Location")]
    public partial class LocationViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database
        readonly ICameraDataService _cameraDataService;
        //Allows configuration files to be utilised by the view model
        readonly IConfiguration _appConfig;
        //A service that allows operations to be performed on locations
        readonly IDeviceLocationDataService _deviceLocationDataService;

        readonly IMotionSensorDataService _motionSensorDataService;

        readonly ILightDataService _lightsDataService;

        readonly IWaterLeakSensorDataService _waterLeakSensorDataService;
        //A collection that the cameras are stored in
        public ObservableCollection<Camera> Cameras { get; } = [];
        //A collection that the motion sensors are stored in
        public ObservableCollection<MotionSensor> MotionSensors { get; } = [];
        //A collection that the lights are stored in
        public ObservableCollection<Light> Lights { get; } = [];
        //A collection that the lights are stored in
        public ObservableCollection<WaterLeakSensor> WaterLeakSensors { get; } = [];

        //An boolean property that determines whether or not the application is in demo mode
        private bool _isInDemoMode;

        [ObservableProperty]
        DeviceLocation location;

        [ObservableProperty]
        public string _updatedlocation;

        [ObservableProperty]
        public bool _isRefreshing;

        //Constructor for the LocationViewModel initialises the various device services
        public LocationViewModel(ICameraDataService cameraDataService, IDeviceLocationDataService deviceLocationDataService, IConfiguration AppConfig, IMotionSensorDataService motionSensorDataService
            ,ILightDataService lightsDataService, IWaterLeakSensorDataService waterLeakSensorDataService)
        {
            _cameraDataService = cameraDataService;
            _deviceLocationDataService = deviceLocationDataService;
            _appConfig = AppConfig;

            Settings appSettings = _appConfig.GetRequiredSection("Settings").Get<Settings>();
            _isInDemoMode = appSettings.DemoMode;
            _motionSensorDataService = motionSensorDataService;
            _lightsDataService = lightsDataService;
            _waterLeakSensorDataService = waterLeakSensorDataService;
        }

        [RelayCommand]
        public async Task GetLocationDevices()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var cameras = await _cameraDataService.GetCamerasAsync(_isInDemoMode);

                Cameras.Clear();

                foreach (var camera in cameras)
                {
                    if(camera.DeviceLocationId.Equals(Location.Id))
                        Cameras.Add(camera);
                }

                var motionSensors = await _motionSensorDataService.GetMotionSensorsAsync(_isInDemoMode);

                MotionSensors.Clear();

                foreach (var sensor in motionSensors)
                {
                    if (sensor.DeviceLocationId.Equals(Location.Id))
                        MotionSensors.Add(sensor);
                }

                var lights = await _lightsDataService.GetLightsAsync(_isInDemoMode);

                Lights.Clear();

                foreach (var light in lights)
                {
                    if (light.DeviceLocationId.Equals(Location.Id))
                        Lights.Add(light);
                }

                var waterLeakSensors = await _waterLeakSensorDataService.GetWaterLeakSensorsAsync(_isInDemoMode);

                WaterLeakSensors.Clear();

                foreach (var waterLeakSensor in waterLeakSensors)
                {
                    if (waterLeakSensor.DeviceLocationId.Equals(Location.Id))
                        WaterLeakSensors.Add(waterLeakSensor);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve devices", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToCreatePageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                //Navigate to the add device page passing the selected location object within a dictionary and a true value for animate
                await Shell.Current.GoToAsync($"{nameof(AddDevice)}", true,
                    new Dictionary<string, object> { { "Location", Location } });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToUpdatePageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                
                await Shell.Current.GoToAsync($"{nameof(UpdateLocation)}", true,
                    new Dictionary<string, object> { { "Location", Location } });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToDeviceDetailsAsync(object selectedObject)
        {
            //Ensure the application is not performing another I/O operation
            if (IsBusy)
                return;
            try
            {
                //Sets the IsBusy flag to true
                IsBusy = true;

                switch (selectedObject)
                {
                    case Camera camera:
                        await Shell.Current.GoToAsync($"{nameof(CameraDetails)}", true,
                            new Dictionary<string, object> { { "Camera", selectedObject } });
                        break;

                    case MotionSensor motionSensor:
                        await Shell.Current.GoToAsync($"{nameof(MotionSensorDetails)}",
                            new Dictionary<string, object> { { "MotionSensor", selectedObject } });
                        break;
                    default:
                        await Shell.Current.DisplayAlert("Invalid","Invalid device","OK");
                        break;
                }
            }
            finally
            {
                //Sets the IsBusy flag to false
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SyncDevicesAsync()
        {
            try
            {
                if (IsBusy)
                    return;
                else
                {
                    await _cameraDataService.SyncCamerasAsync();
                    await _motionSensorDataService.SyncMotionSensorsAsync();
                    await _lightsDataService.SyncLightsAsync();
                    await _waterLeakSensorDataService.SyncWaterLeakSensorsAsync();
                    await GetLocationDevices();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task UpdateLocationAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                Location.LocationName = Updatedlocation;
                await _deviceLocationDataService.UpdateLocationAsync(Location);

                await Shell.Current.GoToAsync("../..");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// The DeleteLocationAsync method removes a location from the local SQLite database
        /// </summary>
        
        [RelayCommand]
        public async Task DeleteLocationAsync()
        {
            //Ensures the application is not performing another I/O operation
            if (IsBusy)
                return;

            try
            {
                //Set the IsBusy flag to true
                IsBusy = true;
                //Remove the location from the local database
                await _deviceLocationDataService.RemoveLocationAsync(Location);
                //Return to the dashboard
                await Shell.Current.GoToAsync("..");
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                //Set the IsBusy flag to false
                IsBusy = false;
            }
        }
    }
}
