using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace SleepSure.ViewModel
{
    [QueryProperty("Location","Location")]
    public partial class LocationViewModel : BaseViewModel
    {
        //A service that retrieves a list of cameras from a local SQLite database
        readonly ICameraDataService _cameraDataService;

        //A collection that the cameras are stored in
        public ObservableCollection<Camera> Cameras { get; } = [];

        [ObservableProperty]
        DeviceLocation location;

        //Constructor for the LocationViewModel initialises the various device services
        public LocationViewModel(ICameraDataService cameraDataService)
        {
            _cameraDataService = cameraDataService;
        }

        [RelayCommand]
        public async Task GetLocationDevices()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var cameras = await _cameraDataService.GetCamerasAsync();
                if (cameras.Count != 0)
                    Cameras.Clear();

                foreach (var camera in cameras)
                {
                    if(camera.LocationId.Equals(Location.Id))
                        Cameras.Add(camera);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve cameras", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
