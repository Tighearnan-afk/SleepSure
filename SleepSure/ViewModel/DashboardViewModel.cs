using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.ViewModel
{
    public class DashboardViewModel : BaseViewModel
    {
        //A service that retrieves a list of devices from either a local json file or an sqlite database
        readonly IDeviceDataService _deviceService;
        //A service that retrieves or adds a list of sensors from/to a local sqlite database
        readonly ISensorDataService _sensorDataService;
        //A collection that the devices are stored in
        public ObservableCollection<Model.Device> Devices { get; } = new ObservableCollection<Model.Device>();

        public ObservableCollection<Sensor> Sensors { get; } = [];
        //Constructor for the DeviceViewModel initialises the DeviceService and the GetDeviceCommand
        public DashboardViewModel(IDeviceDataService deviceService, ISensorDataService sensorDataService)
        {
            _deviceService = deviceService;

            _sensorDataService = sensorDataService;

            GetDevicesCommand = new Command(async () => await GetDevicesAsync());

            AddDeviceCommand = new Command(async () => await TestDB());

            GetSensorsCommand = new Command(async () => await GetSensorsAsync());

            AddSensorCommand = new Command(async () => await TestSensorAdd());
        }
        public Command GetDevicesCommand { get; }
        public Command AddDeviceCommand { get; }

        public Command GetSensorsCommand { get; }
        public Command AddSensorCommand { get; }
        //The GetDevicesAsync method retrieves a list of devices from the devices service and adds them to the Devices collection
        public async Task GetDevicesAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var devices = await _deviceService.GetDevicesAsync();
                if (devices.Count != 0)
                    Devices.Clear();

                foreach (var device in devices)
                {
                    Devices.Add(device);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve devices", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

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

        public async Task TestSensorAdd()
        {
            await _sensorDataService.AddSensorAsync();
        }

        public async Task TestDB()
        {
            await _deviceService.AddDeviceAsync();
            await _deviceService.AddDeviceAsync();
        }
    }
}
