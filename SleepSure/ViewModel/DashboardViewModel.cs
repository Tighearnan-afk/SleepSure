using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Pages;
using SleepSure.Services.DB_Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    public partial class DashboardViewModel : BaseViewModel
    {
        //A service that retrieves a list of locations from a local SQLite database
        readonly IDeviceLocationDataService _locationDataService;

        //A collection that the sensors are stored in
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        //Constructor for the DashboardVIewModel initialises the SensorService
        public DashboardViewModel(IDeviceLocationDataService locationDataService)
        {
            _locationDataService = locationDataService;
        }

        [RelayCommand]
        public async Task GetLocationsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var locations = await _locationDataService.GetLocationsAsync();
                if (locations.Count != 0)
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
        //[RelayCommand]
        //public async Task GetCamerasAsync()
        //{
        //    if (IsBusy)
        //        return;

        //    try
        //    {
        //        IsBusy = true;
        //        var cameras = await _cameraDataService.GetCamerasAsync();
        //        if (cameras.Count != 0)
        //            Cameras.Clear();

        //        foreach (var camera in cameras)
        //        {
        //            Cameras.Add(camera);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        //Display an alert if an exception occurs
        //        await Shell.Current.DisplayAlert("Error", "Unable to retrieve cameras", "OK");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}

        //[RelayCommand]
        //public async Task GetSensorsAsync()
        //{
        //    if (IsBusy)
        //        return;

        //    try
        //    {
        //        IsBusy = true;
        //        var sensors = await _sensorDataService.GetSensorsAsync();
        //        if (sensors.Count != 0)
        //            Sensors.Clear();

        //        foreach (var sensor in sensors)
        //        {
        //            Sensors.Add(sensor);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        //Display an alert if an exception occurs
        //        await Shell.Current.DisplayAlert("Error", "Unable to retrieve sensors", "OK");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}
        //[RelayCommand]
        //public async Task TestSensorAdd()
        //{
        //    await _sensorDataService.AddSensorAsync();
        //    await GetSensorsAsync();
        //}
    }
}
