using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace SleepSure.ViewModel
{
    [QueryProperty("Location", "Location")]
    public partial class AddDeviceViewModel : BaseViewModel
    {
        //A service that retrieves the device types from a local JSON file
        readonly IDeviceTypeService _deviceTypeService;
        //A service that retrieves a list of cameras from a local SQLite database
        readonly ICameraDataService _cameraDataService;

        readonly IMotionSensorDataService _motionSensorDataService;
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];

        public AddDeviceViewModel(IDeviceTypeService DeviceTypeService, ICameraDataService cameraDataService, IMotionSensorDataService motionSensorDataService)
        {
            _deviceTypeService = DeviceTypeService;
            _cameraDataService = cameraDataService;
            _motionSensorDataService = motionSensorDataService;
        }

        [ObservableProperty]
        public DeviceLocation _location;
        [ObservableProperty]
        public string _newDeviceName;
        [ObservableProperty]
        public Model.DeviceType _newDeviceType;
        [ObservableProperty]
        public string _newDeviceDescription;

        [RelayCommand]
        public async Task RetrieveDeviceTypes()
        {
            if (DeviceTypes.Count > 0)
                return;

            try
            {
                var types = await _deviceTypeService.GetTypesAsync();
                foreach (var type in types)
                {
                    DeviceTypes.Add(type);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        /// <summary>
        /// The AddDeviceAsync method add a new device to the SQLite database and REST API based on the current location
        /// </summary>
        [RelayCommand]
        public async Task AddDeviceAsync()
        {
            //Ensure the device name field is not empty
            if (NewDeviceName is null)
                return;
            //Ensure the device type field is not empty
            if (NewDeviceType is null)
                return;
            //Ensure the device description field is not empty
            if (NewDeviceDescription is null)
                return;

            //Ensure the application is not performing another I/O operation
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                //Instanstiate a random object for randomly assigning paramters such as battery life and temperature to the approporiate devices
                Random random = new Random();

                //Choose which device service to use based on the chosen device type
                switch (NewDeviceType.Type)
                {
                    case "Camera":
                        await _cameraDataService.AddCameraAsync(NewDeviceName, NewDeviceDescription, (int)Location.Id);
                        break;
                    case "MotionSensor":
                        await _motionSensorDataService.AddMotionSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    default:
                        return;
                }
                
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
