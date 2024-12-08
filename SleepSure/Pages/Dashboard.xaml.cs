using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Dashboard : ContentPage
{
	public Dashboard(DashboardViewModel viewModel)
	{
		InitializeComponent();

        //Set the binding context to the view model
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();                                                 
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        /*Code retrieved from ChatGPT
        This code overloads the OnAppearing() method found in the Page class and executes the GetLocationsCommand to retrieve
        the device locations before the page appears*/
        if (BindingContext is DashboardViewModel viewModel) 
        {
            viewModel.GetLocationsCommand.Execute(null);
        }
    }
}