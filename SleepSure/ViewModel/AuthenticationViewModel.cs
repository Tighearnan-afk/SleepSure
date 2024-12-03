using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace SleepSure.ViewModel
{
    public partial class AuthenticationViewModel : BaseViewModel
    {
        readonly IUserDataService _userDataService;

        readonly IConfiguration _appConfig;

        private bool _isInDemoMode;

        public ObservableCollection<User> Users { get; } = [];

        public AuthenticationViewModel(IUserDataService userDataService, IConfiguration AppConfig)
        {
            _userDataService = userDataService;
            _appConfig = AppConfig;

            Settings appSettings = _appConfig.GetRequiredSection("Settings").Get<Settings>();
            _isInDemoMode = appSettings.DemoMode;
        }

        [ObservableProperty]
        public string _email;
        [ObservableProperty]
        public string _password;
        [ObservableProperty]
        public string _reenteredPassword;

        [RelayCommand]
        private async Task Login()
        {
            await GetUsersAsync();

            if (Email is null || Password is null)
                return;
            if (Users.Any(x => x.Email.Equals(Email)) && Users.Any(x => x.Password.Equals(Password)))
                await Shell.Current.GoToAsync("//dashboard");
            else
                await Shell.Current.DisplayAlert("Invalid Credentials","You have entered invalid credentials","OK");

        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync("register");
        }

        [RelayCommand]
        private async Task Register()
        {
            if(Email is null || Password is null || ReenteredPassword is null)
            {
                await Shell.Current.DisplayAlert("All fields are required","Please enter an email and confirm password","OK");
                return;
            }

            if(!Password.Equals(ReenteredPassword))
            {
                await Shell.Current.DisplayAlert("Non-matching passwords", "Passwords must match", "OK");
                return;
            }

            await RegisterUserAsync(Email,Password);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task GetUsersAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                var users = await _userDataService.GetUsersAsync(_isInDemoMode);
                if (users.Count != 0)
                    Users.Clear();

                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve users", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task RegisterUserAsync(string email, string password)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await _userDataService.AddUserAsync(email,password);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
