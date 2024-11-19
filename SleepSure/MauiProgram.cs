﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SleepSure.Pages;
using SleepSure.Services;
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

            //Register the database
            builder.Services.AddSingleton<IDeviceDataService, DeviceDBDataService>(
                s => ActivatorUtilities.CreateInstance<DeviceDBDataService>(s, databasePath));

            builder.Services.AddSingleton<ISensorDataService, SensorDBDataService>(
                s => ActivatorUtilities.CreateInstance<SensorDBDataService>(s, databasePath));

            //Register the device service as a singleton with the Dependency Injection service
            builder.Services.AddSingleton<DeviceFileService>();

            //Register the view model with the DI service
            builder.Services.AddSingleton<DeviceViewModel>();
            //Register the dashboard page
            builder.Services.AddSingleton<Dashboard>();

            builder.Services.AddSingleton<DashboardViewModel>();

            return builder.Build();
        }
    }
}
