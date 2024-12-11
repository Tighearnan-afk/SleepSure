using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("HumiditySensor", "HumiditySensor")]
    public partial class HumiditySensorDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly IHumiditySensorDataService _humiditySensorDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public HumiditySensor _humiditySensor;

        public HumiditySensorDetailsViewModel(IHumiditySensorDataService humiditySensorDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _humiditySensorDataService = humiditySensorDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedHumiditySensorLocation;

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
        public async Task UpdateHumiditySensorAsync()
        {
            if (HumiditySensor is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedHumiditySensorLocation is not null)
                    HumiditySensor.DeviceLocationId = (int)UpdatedHumiditySensorLocation.Id;
                await _humiditySensorDataService.UpdateHumiditySensorAsync(HumiditySensor);

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
        public async Task DeleteHumiditySensorAsync()
        {
            if (HumiditySensor is null)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the humidity sensor
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {HumiditySensor.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _humiditySensorDataService.DeleteHumiditySensorAsync(HumiditySensor);

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
