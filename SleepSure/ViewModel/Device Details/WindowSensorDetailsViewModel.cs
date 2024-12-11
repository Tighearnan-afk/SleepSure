using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace SleepSure.ViewModel
{
    [QueryProperty("WindowSensor", "WindowSensor")]
    public partial class WindowSensorDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly IWindowSensorDataService _windowSensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public WindowSensor _windowSensor;

        public WindowSensorDetailsViewModel(IWindowSensorDataService windowSensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _windowSensorDataService = windowSensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedWindowSensorLocation;

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
        public async Task UpdateWindowSensorAsync()
        {
            if (WindowSensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedWindowSensorLocation is not null)
                    WindowSensor.DeviceLocationId = (int)UpdatedWindowSensorLocation.Id;
                await _windowSensorDataService.UpdateWindowSensorAsync(WindowSensor);

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
        public async Task DeleteWindowSensorAsync()
        {
            if (WindowSensor is null)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the window sensor
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {WindowSensor.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _windowSensorDataService.DeleteWindowSensorAsync(WindowSensor);

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
