using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Services;

namespace SleepSure.ViewModel
{
    public partial class AddLocationViewModel : BaseViewModel
    {
        //A service that performs operations on the location table in the local SQLite database, also allows operations to be performed on the locations present in the REST API
        readonly IDeviceLocationDataService _locationDataService;

        //Constructor for the DashboardVIewModel initialises the SensorService
        public AddLocationViewModel(IDeviceLocationDataService locationDataService)
        {
            _locationDataService = locationDataService;
        }

        [ObservableProperty]
        public string _newLocation;

        [RelayCommand]
        public async Task AddLocationAsync()
        {
            if (NewLocation is null)
                return;

            await _locationDataService.AddLocationAsync(NewLocation);

            await _locationDataService.SyncLocationsAsync();

            await Shell.Current.GoToAsync("..");
        }
    }
}
