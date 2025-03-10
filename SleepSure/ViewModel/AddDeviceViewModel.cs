﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
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

        readonly ILightDataService _lightsDataService;

        readonly IWaterLeakSensorDataService _waterLeakSensorDataService;

        readonly IDoorSensorDataService _doorSensorDataServer;

        readonly IWindowSensorDataService _windowSensorDataService;

        readonly ITemperatureSensorDataService _temperatureSensorDataService;

        readonly IHumiditySensorDataService _humiditySensorDataService;

        readonly IVibrationSensorDataService _vibrationSensorDataService;

        //A list that stores the available device types
        public ObservableCollection<Model.DeviceType> DeviceTypes { get; } = [];

        public AddDeviceViewModel(IDeviceTypeService DeviceTypeService, ICameraDataService cameraDataService, IDeviceLocationDataService deviceLocationDataService, IConfiguration AppConfig, IMotionSensorDataService motionSensorDataService,
                                 ILightDataService lightsDataService, IWaterLeakSensorDataService waterLeakSensorDataService, IDoorSensorDataService doorSensorDataServer, IWindowSensorDataService windowSensorDataService,
                                 ITemperatureSensorDataService temperatureSensorDataService, IHumiditySensorDataService humiditySensorDataService, IVibrationSensorDataService vibrationSensorDataService)
        {
            _deviceTypeService = DeviceTypeService;
            _cameraDataService = cameraDataService;
            _motionSensorDataService = motionSensorDataService;
            _lightsDataService = lightsDataService;
            _waterLeakSensorDataService = waterLeakSensorDataService;
            _doorSensorDataServer = doorSensorDataServer;
            _windowSensorDataService = windowSensorDataService;
            _temperatureSensorDataService = temperatureSensorDataService;
            _humiditySensorDataService = humiditySensorDataService;
            _vibrationSensorDataService = vibrationSensorDataService;
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
            if (NewDeviceName is null || NewDeviceName == "")
                return;
            //Ensure the device type field is not empty
            if (NewDeviceType is null)
                return;
            //Ensure the device description field is not empty
            if (NewDeviceDescription is null || NewDeviceDescription == "")
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
                    case "Motion Sensor":
                        await _motionSensorDataService.AddMotionSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    case "Door Sensor":
                        await _doorSensorDataServer.AddDoorSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    case "Window Sensor":
                        await _windowSensorDataService.AddWindowSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    case "Temperature Sensor":
                        await _temperatureSensorDataService.AddTemperatureSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    case "Humidity Sensor":
                        await _humiditySensorDataService.AddHumiditySensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), random.Next(0,100), (int)Location.Id);
                        break;
                    case "Vibration Sensor":
                        await _vibrationSensorDataService.AddVibrationSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    case "Waterleak Sensor":
                        await _waterLeakSensorDataService.AddWaterLeakSensorAsync(NewDeviceName, NewDeviceDescription, random.Next(0, 100), random.Next(0, 32), (int)Location.Id);
                        break;
                    case "Light":
                        await _lightsDataService.AddLightAsync(NewDeviceName, NewDeviceDescription, 100, (int)Location.Id);
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
