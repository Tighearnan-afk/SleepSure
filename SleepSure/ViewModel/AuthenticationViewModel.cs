using Microsoft.Maui.Controls;
using SleepSure.Model;
using SleepSure.Pages;
using SleepSure.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.ViewModel
{
    public class AuthenticationViewModel : BaseViewModel
    {
        readonly IUserDataService _userDataService;

        public ObservableCollection<User> Users { get; } = [];

        public AuthenticationViewModel(IUserDataService userDataService)
        {
            _userDataService = userDataService;

            GetUsersCommand = new Command(async () => await GetUsersAsync());

            RegisterUserCommand = new Command(async () => await RegisterUserAsync());

            LoginCommand = new Command(async () => await Login());
            GoToRegisterCommand = new Command(async () => await GoToRegister());
            RegisterCommand = new Command(async () => await Register());
        }

        public Command GetUsersCommand { get; }
        public Command RegisterUserCommand { get; }
        public Command LoginCommand { get; }
        public Command GoToRegisterCommand { get; }
        public Command RegisterCommand { get; }

        private async Task Login()
        {
            await Shell.Current.GoToAsync("//dashboard");
        }

        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync("register");
        }

        private async Task Register()
        {
            await _userDataService.AddUserAsync();
            await Shell.Current.GoToAsync("..");
        }

        public async Task GetUsersAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                var users = await _userDataService.GetUsersAsync();
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

        public async Task RegisterUserAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await _userDataService.AddUserAsync();
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
