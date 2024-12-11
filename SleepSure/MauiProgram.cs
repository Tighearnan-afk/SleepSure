using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SleepSure.Pages;
using SleepSure.Services;
using SleepSure.Services.REST_Services;
using SleepSure.ViewModel;
using System.Reflection;
using UraniumUI;

namespace SleepSure
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular"); //Retrieved from https://fonts.google.com/specimen/Roboto
                    fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
                    fonts.AddMaterialSymbolsFonts();
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            //Create a configuration builder and add the appsettings to the application
            //Get an instance of the executing assembly where the appsettings is embedded
            var assembly = Assembly.GetExecutingAssembly();

            //Used for finding the path of the applications resources
            //var resourceLocations = assembly.GetManifestResourceNames();

            using var stream = assembly.GetManifestResourceStream("SleepSure.Resources.Raw.appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            //Add the configuration to the application
            builder.Configuration.AddConfiguration(configuration);

            //Get the name of the database
            var databaseName = configuration.GetSection("Settings").Get<Model.Settings>().DatabaseName;

            //Get the filepath to the database for every OS
            string databasePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, databaseName);

            //Register the REST services
            builder.Services.AddSingleton<IDeviceLocationRESTService, DeviceLocationRESTService>();
            builder.Services.AddSingleton<ICameraRESTService, CameraRESTService>();
            builder.Services.AddSingleton<IUserRESTService, UserRESTService>();
            builder.Services.AddSingleton<IMotionSensorRESTService, MotionSensorRESTService>();
            builder.Services.AddSingleton<ILightRESTService, LightRESTService>();
            builder.Services.AddSingleton<IWaterLeakSensorRESTService, WaterLeakSensorRESTService>();
            builder.Services.AddSingleton<IDoorSensorRESTService, DoorSensorRESTService>();
            builder.Services.AddSingleton<IWindowSensorRESTService, WindowSensorRESTService>();
            builder.Services.AddSingleton<ITemperatureSensorRESTService, TemperatureSensorRESTService>();
            builder.Services.AddSingleton<IHumiditySensorRESTService, HumiditySensorRESTService>();
            builder.Services.AddSingleton<IVibrationSensorRESTService, VibrationSensorRESTService>();

            //Register the database tables
            builder.Services.AddSingleton<IDeviceLocationDataService, DeviceLocationDBService>(
                s => ActivatorUtilities.CreateInstance<DeviceLocationDBService>(s, databasePath));

            builder.Services.AddSingleton<IUserDataService, UserDBDataService>(
                s => ActivatorUtilities.CreateInstance<UserDBDataService>(s, databasePath));

            builder.Services.AddSingleton<ICameraDataService, CameraDBDataService>(
                s => ActivatorUtilities.CreateInstance<CameraDBDataService>(s, databasePath));

            builder.Services.AddSingleton<IVideoDataService, VideoDBDataService>(
                s => ActivatorUtilities.CreateInstance<VideoDBDataService>(s, databasePath));

            builder.Services.AddSingleton<IMotionSensorDataService, MotionSensorDBDataService>(
                s => ActivatorUtilities.CreateInstance<MotionSensorDBDataService>(s, databasePath));

            builder.Services.AddSingleton<ILightDataService, LightDBDataService>(
                s => ActivatorUtilities.CreateInstance<LightDBDataService>(s, databasePath));

            builder.Services.AddSingleton<IWaterLeakSensorDataService, WaterLeakSensorDBService>(
                s => ActivatorUtilities.CreateInstance<WaterLeakSensorDBService>(s, databasePath));

            builder.Services.AddSingleton<IDoorSensorDataService, DoorSensorDBService>(
                s => ActivatorUtilities.CreateInstance<DoorSensorDBService>(s, databasePath));

            builder.Services.AddSingleton<IWindowSensorDataService, WindowSensorDBDataService>(
                s => ActivatorUtilities.CreateInstance<WindowSensorDBDataService>(s, databasePath));

            builder.Services.AddSingleton<ITemperatureSensorDataService, TemperatureSensorDataService>(
                s => ActivatorUtilities.CreateInstance<TemperatureSensorDataService>(s, databasePath));

            builder.Services.AddSingleton<IHumiditySensorDataService, HumiditySensorDBService>(
                s => ActivatorUtilities.CreateInstance<HumiditySensorDBService>(s, databasePath));

            builder.Services.AddSingleton<IVibrationSensorDataService, VibrationSensorDBService>(
                s => ActivatorUtilities.CreateInstance<VibrationSensorDBService>(s, databasePath));

            builder.Services.AddSingleton<IAlarmDataService, AlarmDBDataService>(
                s => ActivatorUtilities.CreateInstance<AlarmDBDataService>(s, databasePath));

            //Register the devicetype file service
            builder.Services.AddSingleton<IDeviceTypeService, DeviceTypeFileService>();

            //Register the login page
            builder.Services.AddSingleton<Login>();
            //Register the register page
            builder.Services.AddSingleton<Register>();
            //Register the authentication viewmodel
            builder.Services.AddSingleton<AuthenticationViewModel>();
            //Register the dashboard page
            builder.Services.AddSingleton<Dashboard>();
            //Register the dashboard viewmodel
            builder.Services.AddSingleton<DashboardViewModel>();
            //Register the location page
            builder.Services.AddTransient<LocationPage>();
            //Register the location viewmodel
            builder.Services.AddTransient<LocationViewModel>();
            //Register the add location page
            builder.Services.AddSingleton<AddLocation>();
            //Register the add location viewmodel
            builder.Services.AddSingleton<AddLocationViewModel>();
            //Register the update location page
            builder.Services.AddTransient<UpdateLocation>();
            //Register the add device page
            builder.Services.AddTransient<AddDevice>();
            //Register the add device viewmodel
            builder.Services.AddTransient<AddDeviceViewModel>();
            //Register the security page
            builder.Services.AddSingleton<Pages.Security>();
            //Register the security view model
            builder.Services.AddSingleton<SecurityViewModel>();
            //Register the video feed page
            builder.Services.AddTransient<VideoFeed>();
            //Register the video feed view model
            builder.Services.AddTransient<VideoFeedViewModel>();
            //Register the video archive page 
            builder.Services.AddTransient<VideoArchive>();
            //Register the video archive view model
            builder.Services.AddTransient<VideoArchiveViewModel>();
            //Register the notification page 
            builder.Services.AddSingleton<Notifications>();
            //Register the notification view model
            builder.Services.AddSingleton<NotificationViewModel>();

            //Device Details
            //Register the camera details page 
            builder.Services.AddTransient<CameraDetails>();
            //Register the camera details view model 
            builder.Services.AddTransient<CameraDetailsViewModel>();
            //Register the motion sensor details details page 
            builder.Services.AddTransient<MotionSensorDetails>();
            //Register the light details view model 
            builder.Services.AddTransient<MotionDetailsSensorViewModel>();
            //Register the light details details page 
            builder.Services.AddTransient<LightDetails>();
            //Register the light details view model 
            builder.Services.AddTransient<LightDetailsViewModel>();
            //Register the waterleak sensor details details page 
            builder.Services.AddTransient<WaterLeakSensorDetails>();
            //Register the waterleak sensor details view model 
            builder.Services.AddTransient<WaterLeakSensorDetailsViewModel>();
            //Register the door sensor details details page 
            builder.Services.AddTransient<DoorSensorDetails>();
            //Register the door sensor details view model 
            builder.Services.AddTransient<DoorSensorDetailsViewModel>();
            //Register the window sensor details details page 
            builder.Services.AddTransient<WindowSensorDetails>();
            //Register the window sensor details view model 
            builder.Services.AddTransient<WindowSensorDetailsViewModel>();
            //Register the humidity sensor details details page 
            builder.Services.AddTransient<HumiditySensorDetails>();
            //Register the humidity sensor details view model 
            builder.Services.AddTransient<HumiditySensorDetailsViewModel>();
            ////Register the temperature sensor details details page 
            builder.Services.AddTransient<TemperatureSensorDetails>();
            //Register the temperature sensor details view model 
            builder.Services.AddTransient<TemperatureSensorDetailsViewModel>();
            //Register the vibration sensor details details page 
            builder.Services.AddTransient<VibrationSensorDetails>();
            //Register the vibration sensor details view model 
            builder.Services.AddTransient<VibrationSensorDetailsViewModel>();

            return builder.Build();
        }
    }
}
