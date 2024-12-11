using SleepSure.Pages;

namespace SleepSure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register routes
            Routing.RegisterRoute(nameof(Register), typeof(Register));
            Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
            Routing.RegisterRoute(nameof(LocationPage), typeof(LocationPage));
            Routing.RegisterRoute(nameof(AddLocation), typeof(AddLocation));
            Routing.RegisterRoute(nameof(UpdateLocation), typeof(UpdateLocation));
            Routing.RegisterRoute(nameof(AddDevice), typeof(AddDevice));
            Routing.RegisterRoute(nameof(VideoFeed), typeof(VideoFeed));
            Routing.RegisterRoute(nameof(VideoArchive), typeof(VideoArchive));

            //Device Configuration Routes
            Routing.RegisterRoute(nameof(CameraDetails), typeof(CameraDetails));
            Routing.RegisterRoute(nameof(MotionSensorDetails), typeof(MotionSensorDetails));
            Routing.RegisterRoute(nameof(LightDetails), typeof(LightDetails));
            Routing.RegisterRoute(nameof(WaterLeakSensorDetails), typeof(WaterLeakSensorDetails));
            Routing.RegisterRoute(nameof(DoorSensorDetails), typeof(DoorSensorDetails));
            Routing.RegisterRoute(nameof(WindowSensorDetails), typeof(WindowSensorDetails));
            Routing.RegisterRoute(nameof(HumiditySensorDetails), typeof(HumiditySensorDetails));
            Routing.RegisterRoute(nameof(TemperatureSensorDetails), typeof(TemperatureSensorDetails));
            Routing.RegisterRoute(nameof(VibrationSensorDetails), typeof(VibrationSensorDetails));
        }
    }
}
