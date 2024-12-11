using SleepSure.ViewModel;
namespace SleepSure.Pages;

public partial class TemperatureSensorDetails : ContentPage
{
    public TemperatureSensorDetails(TemperatureSensorDetailsViewModel viewmodel)
    {
        InitializeComponent();

        BindingContext = viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is TemperatureSensorDetailsViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}