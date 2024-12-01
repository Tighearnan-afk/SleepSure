using SleepSure.Pages;

namespace SleepSure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register routes
            Routing.RegisterRoute("register", typeof(Register));
            Routing.RegisterRoute("dashboard", typeof(Dashboard));
            Routing.RegisterRoute(nameof(LocationPage), typeof(LocationPage));
            Routing.RegisterRoute("devicedetails", typeof(DeviceDetails));
            Routing.RegisterRoute("adddevice", typeof(AddDevice));
            Routing.RegisterRoute("videofeed", typeof(VideoFeed));
        }
    }
}
