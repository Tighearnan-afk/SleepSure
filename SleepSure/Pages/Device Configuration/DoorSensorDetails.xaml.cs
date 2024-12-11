using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class DoorSensorDetails : ContentPage
{
    public DoorSensorDetails(DoorSensorDetailsViewModel viewmodel)
    {
        InitializeComponent();

        BindingContext = viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is DoorSensorDetailsViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}