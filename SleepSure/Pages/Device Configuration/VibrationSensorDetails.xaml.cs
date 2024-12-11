using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class VibrationSensorDetails : ContentPage
{
    public VibrationSensorDetails(VibrationSensorDetailsViewModel viewmodel)
    {
        InitializeComponent();

        BindingContext = viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is VibrationSensorDetailsViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}