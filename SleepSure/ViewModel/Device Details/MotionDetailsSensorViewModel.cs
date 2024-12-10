using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("MotionSensor", "MotionSensor")]
    public partial class MotionDetailsSensorViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly IMotionSensorDataService _motionSensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public MotionSensor _motionSensor;

        public MotionDetailsSensorViewModel(IMotionSensorDataService motionSensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _motionSensorDataService = motionSensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedMotionSensorLocation;

        [RelayCommand]
        public async Task RetrieveLocations()
        {
            if (Locations.Count > 0)
                return;

            try
            {
                var locations = await _deviceLocationDataService.GetLocationsAsync(false);
                foreach (var location in locations)
                {
                    Locations.Add(location);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [RelayCommand]
        public async Task UpdateMotionSensorAsync()
        {
            if (MotionSensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedMotionSensorLocation is not null)
                    MotionSensor.DeviceLocationId = (int)UpdatedMotionSensorLocation.Id;
                await _motionSensorDataService.UpdateMotionSensorAsync(MotionSensor);

            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally
            {
                IsBusy = false;
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        public async Task DeleteMotionSensorAsync()
        {
            if (MotionSensor is null)
                return;

            try
            {
                IsBusy = true;
                await _motionSensorDataService.DeleteMotionSensorAsync(MotionSensor);

            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally
            {
                IsBusy = false;
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
