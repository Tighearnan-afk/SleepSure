using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Devices.Sensors;
using SleepSure.Model;
using SleepSure.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    public partial class NotificationViewModel : BaseViewModel
    {
        //A service that retrieves a list of alarms stored in the local SQLite database
        readonly IAlarmDataService _alarmDataService;
        //An observable collection that stores alarms
        public ObservableCollection<Alarm> Alarms { get; set; } = [];
        //Constructor for the DashboardVIewModel initialises the alarm service
        public NotificationViewModel(IAlarmDataService alarmDataService)
        {
            _alarmDataService = alarmDataService;
        }

        [ObservableProperty]
        public bool _isRefreshing;

        [RelayCommand]
        public async Task GetAlarmsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var alarms = await _alarmDataService.GetAlarmsAsync();

                Alarms.Clear();

                foreach (var alarm in alarms)
                {
                    Alarms.Add(alarm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to retrieve locations", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task DeleteAlarmAsync(Alarm alarm)
        {
            //Ensures the application is not performing another I/O operation
            if (IsBusy)
                return;

            try
            {
                //Display an alert to confirm the user wishes to delete the alarm
                var result = await Shell.Current.DisplayAlert("Confirm", $"Are you sure you want to delete {alarm.EventName}", "Yes", "No");
                //If the answer is no then return
                if (result == false)
                    return;

                //Sets the busy flag to true
                IsBusy = true;

                await _alarmDataService.RemoveAlarmAsync(alarm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Display an alert if an exception occurs
                await Shell.Current.DisplayAlert("Error", "Unable to delete alarm", "OK");
            }
            finally
            {
                IsBusy = false;
                await GetAlarmsAsync();
            }
        }
    }
}
