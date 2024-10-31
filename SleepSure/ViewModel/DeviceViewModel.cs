using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    public class DeviceViewModel : BaseViewModel
    {
        //A service that retrieves a list of devices from a local json file
         readonly DeviceService _deviceService;

        //A collection that the devices are stored in
        public ObservableCollection<Model.Device> Devices { get; } = new ObservableCollection<Model.Device>();

        //Create a GetDevices command that the view can use to retrieve the list of devices
        public Command GetDevicesCommand { get; }

        //Constructor for the DeviceViewModel initialises the DeviceService and the GetDeviceCommand
        public DeviceViewModel(DeviceService deviceService)
        {
            _deviceService = deviceService;

            //Initialise the GetDevice command to tell it to call the GetDevice Task 
            GetDevicesCommand = new Command(async () => await GetDevicesAsync());
        }

        //The GetDevicesAsync method retrieves a list of devices from the devices service and adds them to the Devices collection
        public async Task GetDevicesAsync()
        { 
            if(IsBusy) 
                return;

            try
            {
                IsBusy = true;
                var devices = await _deviceService.GetDevicesFileAsync();
                if(devices.Count != 0)
                    Devices.Clear();

                foreach(var device in devices)
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
    }
}
