using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class WaterLeakSensorDetails : ContentPage
{
	public WaterLeakSensorDetails(WaterLeakSensorDetailsViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is MotionDetailsSensorViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}