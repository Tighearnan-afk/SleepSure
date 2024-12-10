using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class MotionSensorDetails : ContentPage
{
	public MotionSensorDetails(MotionDetailsSensorViewModel viewmodel)
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