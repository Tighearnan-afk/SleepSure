using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("WaterLeakSensor", "WaterLeakSensor")]
    public partial class WaterLeakSensorDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly IWaterLeakSensorDataService _waterLeakSensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public WaterLeakSensor _waterLeakSensor;

        public WaterLeakSensorDetailsViewModel(IWaterLeakSensorDataService waterLeakSensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _waterLeakSensorDataService = waterLeakSensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedWaterLeakSensorLocation;

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
            if (WaterLeakSensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedWaterLeakSensorLocation is not null)
                    WaterLeakSensor.DeviceLocationId = (int)UpdatedWaterLeakSensorLocation.Id;
                await _waterLeakSensorDataService.UpdateWaterLeakSensorAsync(WaterLeakSensor);

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
            if (WaterLeakSensor is null)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the waterleak sensor
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {WaterLeakSensor.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _waterLeakSensorDataService.DeleteWaterLeakSensorAsync(WaterLeakSensor);

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
