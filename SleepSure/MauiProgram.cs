using Microsoft.Extensions.Logging;
using SleepSure.Pages;
using SleepSure.Services;
using SleepSure.ViewModel;
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

            //Register the device service as a singleton with the Dependency Injection service
            builder.Services.AddSingleton<DeviceService>();
            //Register the view model with the DI service
            builder.Services.AddSingleton<DeviceViewModel>();
            //Register the dashboard page
            builder.Services.AddSingleton<Dashboard>();

            return builder.Build();
        }
    }
}
