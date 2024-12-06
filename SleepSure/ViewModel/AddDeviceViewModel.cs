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
        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];

        //Constructor for the AddDeviceViewModel initialises the Add
        public AddDeviceViewModel(IDeviceTypeService DeviceTypeService, ICameraDataService cameraDataService)
        {
            _deviceTypeService = DeviceTypeService;
            _cameraDataService = cameraDataService;
        }

        [ObservableProperty]
        public DeviceLocation _location;
        [ObservableProperty]
        public string _newDeviceName;
        [ObservableProperty]
        public BaseDevice _newDeviceType;
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

            }
        }

        [RelayCommand]
        public async Task AddDeviceAsync()
        {
            if (NewDeviceName is null)
                return;

            try
            {
                IsBusy = true;
                await _cameraDataService.AddCameraAsync(NewDeviceName, NewDeviceDescription, (int)Location.Id);
                
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
