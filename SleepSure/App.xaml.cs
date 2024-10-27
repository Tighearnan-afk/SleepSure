using SleepSure.Pages;

namespace SleepSure
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new Login();
        }
    }
}
