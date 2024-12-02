using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class LocationPage : ContentPage
{
	public LocationPage(LocationViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}
    //Executes when the pages loads
    protected override void OnAppearing()
    {
        base.OnAppearing();
        //Makes the tab bar invisible when the page is navigated to
        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);
        //Checks if the view model is the location view model
        if (BindingContext is LocationViewModel viewModel)
        {
            //Executes the command for retrieving the devices associated with the current location
            viewModel.GetLocationDevicesCommand.Execute(null);
        }
    }
}