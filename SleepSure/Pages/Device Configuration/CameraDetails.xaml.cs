using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class CameraDetails : ContentPage
{
	public CameraDetails(CameraDetailsViewModel cameraDetails)
	{
		InitializeComponent();

        BindingContext = cameraDetails;
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