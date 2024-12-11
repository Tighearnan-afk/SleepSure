using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("TemperatureSensor", "TemperatureSensor")]
    public partial class TemperatureSensorDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly ITemperatureSensorDataService _temperatureSensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public TemperatureSensor _temperatureSensor;

        public TemperatureSensorDetailsViewModel(ITemperatureSensorDataService temperatureSensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _temperatureSensorDataService = temperatureSensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedTemperatureSensorLocation;

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
        public async Task UpdateTemperatureSensorAsync()
        {
            if (TemperatureSensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedTemperatureSensorLocation is not null)
                    TemperatureSensor.DeviceLocationId = (int)UpdatedTemperatureSensorLocation.Id;
                await _temperatureSensorDataService.UpdateTemperatureSensorAsync(TemperatureSensor);

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
        public async Task DeleteTemperatureSensorAsync()
        {
            if (TemperatureSensor is null)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the temperature sensor
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {TemperatureSensor.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _temperatureSensorDataService.DeleteTemperatureSensorAsync(TemperatureSensor);

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
