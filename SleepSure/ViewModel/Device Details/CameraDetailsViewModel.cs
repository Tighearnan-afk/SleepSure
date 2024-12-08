using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("Camera", "Camera")]
    public partial class CameraDetailsViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database 
        readonly ICameraDataService _cameraDataService;
        //A service that retrieves a list of device locations from a local SQLite database
        readonly IDeviceLocationDataService _deviceLocationDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];
        //A list that stores the available device locations
        public ObservableCollection<DeviceLocation> Locations { get; } = [];

        [ObservableProperty]
        public Camera _camera;

        public CameraDetailsViewModel (ICameraDataService cameraDataService, IDeviceLocationDataService deviceLocationDataService)
        {
            _cameraDataService = cameraDataService;
            _deviceLocationDataService = deviceLocationDataService;
        }

        [ObservableProperty]
        public DeviceLocation _updatedCameraLocation;

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
        public async Task UpdateCameraAsync()
        {
            if (Camera is null)
                return;

            try
            {
                IsBusy = true;
                if(UpdatedCameraLocation is not null)
                    Camera.DeviceLocationId = (int)UpdatedCameraLocation.Id;
                await _cameraDataService.UpdateCameraAsync(Camera);

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
        public async Task DeleteCameraAsync()
        {
            if (Camera is null)
                return;

            try
            {
                IsBusy = true;
                await _cameraDataService.DeleteCameraAsync(Camera);

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
