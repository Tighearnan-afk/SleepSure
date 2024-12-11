using SleepSure.ViewModel;

namespace SleepSure.Pages.Device_Configuration;

public partial class LightDetails : ContentPage
{
	public LightDetails(LightDetailsViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is CameraDetailsViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}