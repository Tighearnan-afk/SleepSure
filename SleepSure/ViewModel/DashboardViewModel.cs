using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using SleepSure.Model;
using SleepSure.Pages;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;

namespace SleepSure.ViewModel
{
    public partial class DashboardViewModel : BaseViewModel
    {
        //A service that retrieves a list of locations from a local SQLite database
        readonly IDeviceLocationDataService _locationDataService;
        //Allows configuration files to be utilised by the view model
        readonly IConfiguration _appConfig;
        //A collection that the locations are stored in
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        //An boolean property that determines whether or not the application is in demo mode
        private bool _isInDemoMode;

        [ObservableProperty]
        public bool _isRefreshing;

        //Constructor for the DashboardVIewModel initialises the SensorService
        public DashboardViewModel(IDeviceLocationDataService locationDataService, IConfiguration AppConfig)
        {
            _locationDataService = locationDataService;
            _appConfig = AppConfig;

            Settings appSettings = _appConfig.GetRequiredSection("Settings").Get<Settings>();
            _isInDemoMode = appSettings.DemoMode;
        }

        [RelayCommand]
        public async Task GetLocationsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var locations = await _locationDataService.GetLocationsAsync(_isInDemoMode);
                //if (locations.Count != 0)
                Locations.Clear();

                foreach (var location in locations)
                {
                    Locations.Add(location);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve locations", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        public async Task SyncLocationsAsync()
        {
            try
            {
                if (IsBusy)
                    return;
                else
                {
                    await _locationDataService.SyncLocationsAsync();
                    await GetLocationsAsync();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task GoToLocationAsync(DeviceLocation location)
        {
            if (location is null)
                return;
            //Navigate to the location room passing the selected location object within a dictionary and a true value for animate
            await Shell.Current.GoToAsync($"{nameof(LocationPage)}",true,
                new Dictionary<string, object> { { "Location", location} });
        }

        [RelayCommand]
        public async Task GoToAddLocationAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddLocation),true);
        }
    }
}
