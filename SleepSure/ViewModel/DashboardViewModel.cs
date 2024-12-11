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
        //A boolean property that determines if the page is refreshing
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
                IsRefreshing = false;
            }
        }
        /// <summary>
        /// The SyncLocationsAsync method syncs the devicelocations between the local SQLite database and the RESTAPI
        /// </summary>
        
        [RelayCommand]
        public async Task SyncLocationsAsync()
        {
            //Ensure the application isn't performing another I/O operation
            if (IsBusy)
                return;
            try
            {
                //Set the IsBusy flag to true
                IsBusy = true;
                //Sync the locations between the database and REST API
                await _locationDataService.SyncLocationsAsync();
                //Refresh the Locations observable collection
                await GetLocationsAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                //Set the IsBusy flag to false
                IsBusy = false;
                //Set the IsRefreshing flag to false
                IsRefreshing = false;
            }
        }
        /// <summary>
        /// The GoToLocation method navigates to an individual room page passing a dictionary containing the devicelocation object
        /// </summary>
        /// <param name="location"></param>

        [RelayCommand]
        public async Task GoToLocationAsync(DeviceLocation location)
        {
            //Ensure the location is not null
            if (location is null)
                return;
            //Navigate to the location room passing the selected location object within a dictionary and a true value for animate
            await Shell.Current.GoToAsync($"{nameof(LocationPage)}",true,
                new Dictionary<string, object> { { "Location", location} });
        }

        /// <summary>
        /// The GoToAddLocation method navigates to the AddLocation page
        /// </summary>

        [RelayCommand]
        public async Task GoToAddLocationAsync()
        {
            //Navigate to the AddLocation page passing a true value for animate
            await Shell.Current.GoToAsync(nameof(AddLocation),true);
        }
    }
}
