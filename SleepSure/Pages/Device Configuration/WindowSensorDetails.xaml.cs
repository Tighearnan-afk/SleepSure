using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class WindowSensorDetails : ContentPage
{
    public WindowSensorDetails(WindowSensorDetailsViewModel viewmodel)
    {
        InitializeComponent();

        BindingContext = viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is WindowSensorDetailsViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}