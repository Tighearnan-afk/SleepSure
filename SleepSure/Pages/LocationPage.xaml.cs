using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class LocationPage : ContentPage
{
	public LocationPage(LocationViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        if (BindingContext is LocationViewModel viewModel)
        {
            viewModel.GetLocationDevicesCommand.Execute(null);
        }
    }
}