﻿using SleepSure.Pages;

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
            Routing.RegisterRoute(nameof(AddLocation), typeof(AddLocation));
            Routing.RegisterRoute(nameof(UpdateLocation), typeof(UpdateLocation));
            Routing.RegisterRoute("devicedetails", typeof(DeviceDetails));
            Routing.RegisterRoute(nameof(AddDevice), typeof(AddDevice));
            Routing.RegisterRoute(nameof(VideoFeed), typeof(VideoFeed));
            Routing.RegisterRoute(nameof(VideoArchive), typeof(VideoArchive));
        }
    }
}
