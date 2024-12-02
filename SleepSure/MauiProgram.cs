﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SleepSure.Pages;
using SleepSure.Services;
using SleepSure.Services.DB_Services;
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

            //Register the database tables
            builder.Services.AddSingleton<IDeviceLocationDataService, DeviceLocationDBService>(
                s => ActivatorUtilities.CreateInstance<DeviceLocationDBService>(s, databasePath));

            builder.Services.AddSingleton<IUserDataService, UserDBDataService>(
                s => ActivatorUtilities.CreateInstance<UserDBDataService>(s, databasePath));

            builder.Services.AddSingleton<ICameraDataService, CameraDBDataService>(
                s => ActivatorUtilities.CreateInstance<CameraDBDataService>(s, databasePath));

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

            return builder.Build();
        }
    }
}
