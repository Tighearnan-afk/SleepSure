using SleepSure.Pages;

namespace SleepSure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register routes
            Routing.RegisterRoute("lightdevicedetails", typeof(LightDeviceDetails));
            Routing.RegisterRoute("thermostatdevicedetails", typeof(ThermostatDeviceDetails));
        }
    }
}
