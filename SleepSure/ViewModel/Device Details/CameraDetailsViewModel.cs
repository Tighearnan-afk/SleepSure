using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Pages;
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
                //Display an alert to confirm the user wishes to delete the camera
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {Camera.Name}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

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

        [RelayCommand]
        public async Task GoToVideoArchiveAsync()
        {
            if (Camera is null)
                return;

            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                //Navigate to the video archive page passing the selected camera object within a dictionary and a true value for animate
                await Shell.Current.GoToAsync($"{nameof(VideoArchive)}", true,
                    new Dictionary<string, object> { { "Camera", Camera } });
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
