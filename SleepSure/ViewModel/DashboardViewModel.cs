using CommunityToolkit.Mvvm.Input;
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
        //A service that retrieves a list of devices from a local json file
        readonly IDeviceDataService _deviceService;

        //A collection that the devices are stored in
        public ObservableCollection<Model.Device> Devices { get; } = new ObservableCollection<Model.Device>();
        //Constructor for the DeviceViewModel initialises the DeviceService and the GetDeviceCommand
        public DashboardViewModel(IDeviceDataService deviceService)
        {
            _deviceService = deviceService;

            GetDevicesCommand = new Command(async () => await GetDevicesAsync());

            AddDevicesCommand = new Command(async () => await TestDB());
        }
        public Command GetDevicesCommand { get; }
        public Command AddDevicesCommand { get; }
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

        public async Task TestDB()
        {
            await _deviceService.AddDeviceAsync();
            await _deviceService.AddDeviceAsync();
        }
    }
}
