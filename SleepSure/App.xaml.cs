using SleepSure.Pages;
using SleepSure.ViewModel;

namespace SleepSure
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
