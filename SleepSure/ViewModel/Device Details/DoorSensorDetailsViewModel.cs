using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("DoorSensor", "DoorSensor")]
    public partial class DoorSensorDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly IDoorSensorDataService _doorSensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public DoorSensor _doorSensor;

        public DoorSensorDetailsViewModel(IDoorSensorDataService doorSensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _doorSensorDataService = doorSensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedDoorSensorLocation;

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
        public async Task UpdateDoorSensorAsync()
        {
            if (DoorSensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedDoorSensorLocation is not null)
                    DoorSensor.DeviceLocationId = (int)UpdatedDoorSensorLocation.Id;
                await _doorSensorDataService.UpdateDoorSensorAsync(DoorSensor);

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
        public async Task DeleteDoorSensorAsync()
        {
            if (DoorSensor is null)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the door sensor
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {DoorSensor.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _doorSensorDataService.DeleteDoorSensorAsync(DoorSensor);

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
