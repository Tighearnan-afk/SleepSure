using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("Light", "Light")]
    public partial class LightDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of lights from a local SQLite database 
        readonly ILightDataService _lightDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public Light _light;

        public LightDetailsViewModel(ILightDataService lightDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _lightDataService = lightDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedLightLocation;

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
        public async Task UpdateLightAsync()
        {
            if (Light is null)
                return;

            try
            {
                IsBusy = true;
                if (UpdatedLightLocation is not null)
                    Light.DeviceLocationId = (int)UpdatedLightLocation.Id;
                await _lightDataService.UpdateLightAsync(Light);

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
        public async Task DeleteLightAsync()
        {
            if (Light is null)
                return;

            try
            {

                //Display an alert to confirm the user wishes to delete the light
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {Light.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                IsBusy = true;
                await _lightDataService.DeleteLightAsync(Light);

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
