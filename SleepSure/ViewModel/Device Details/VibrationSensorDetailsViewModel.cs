using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("VibrationSensor", "VibrationSensor")]
    public partial class VibrationSensorDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly IVibrationSensorDataService _vibrationSensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public VibrationSensor _vibrationSensor;

        public VibrationSensorDetailsViewModel(IVibrationSensorDataService vibrationSensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _vibrationSensorDataService = vibrationSensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedVibrationSensorLocation;

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
        public async Task UpdateVibrationSensorAsync()
        {
            if (VibrationSensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedVibrationSensorLocation is not null)
                    VibrationSensor.DeviceLocationId = (int)UpdatedVibrationSensorLocation.Id;
                await _vibrationSensorDataService.UpdateVibrationSensorAsync(VibrationSensor);

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
        public async Task DeleteVibrationSensorAsync()
        {
            if (VibrationSensor is null)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the vibration sensor
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {VibrationSensor.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _vibrationSensorDataService.DeleteVibrationSensorAsync(VibrationSensor);

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
