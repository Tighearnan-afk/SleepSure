using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    public partial class DashboardViewModel : BaseViewModel
    {
        //A service that retrieves or adds a list of sensors from/to a local sqlite database
        readonly ISensorDataService _sensorDataService;

        //A service that retrieves a list of cameras from a REST API
        readonly ICameraRESTService _cameraRESTService;

        //A collection that the sensors are stored in
        public ObservableCollection<Sensor> Sensors { get; } = [];

        public ObservableCollection<Camera> Cameras { get; } = [];
        //Constructor for the DashboardVIewModel initialises the SensorService
        public DashboardViewModel(ISensorDataService sensorDataService, ICameraRESTService cameraRESTService)
        {
            _sensorDataService = sensorDataService;
            _cameraRESTService = cameraRESTService;
        }

        /* [RelayCommand]
        public async Task Appearing()
        {
            await GetSensorsAsync();
        }  Doesnt work wooooooooo */
        //The GetDevicesAsync method retrieves a list of devices from the devices service and adds them to the Devices collection

        [RelayCommand]
        public async Task GetCamerasAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var cameras = await _cameraRESTService.RefreshCamerasAsync();
                if (cameras.Count != 0)
                    Cameras.Clear();

                foreach (var camera in cameras)
                {
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

        [RelayCommand]
        public async Task GetSensorsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var sensors = await _sensorDataService.GetSensorsAsync();
                if (sensors.Count != 0)
                    Sensors.Clear();

                foreach (var sensor in sensors)
                {
                    Sensors.Add(sensor);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve sensors", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        public async Task TestSensorAdd()
        {
            await _sensorDataService.AddSensorAsync();
            await GetSensorsAsync();
        }
    }
}
