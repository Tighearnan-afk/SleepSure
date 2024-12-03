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
        if (BindingContext is DashboardViewModel viewModel)
        {
            viewModel.GetLocationsCommand.Execute(null);
            //viewModel.SyncLocationsCommand.Execute(null);
        }
    }

    //protected override void OnAppearing() //Code retrieved from ChatGPT with the prompt "how do i call an mvvm command when the page load with .net maui"
    //{                                     //This code overloads the OnAppearing() method found in the Page class and executes the GetDevicesCommand to retrieve
    //    base.OnAppearing();               //the users devices before the page appears

    //    if (BindingContext is DeviceViewModel viewModel)
    //    {
    //        viewModel.GetDevicesCommand.Execute(null);
    //    }
    //}

    public async void OnBtnAddDevice_Clicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("adddevice");
    }

    public async void OnTapGestureRecogniserTappedLight(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }

    public async void OnTapGestureRecogniserTappedThermostat(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }

    public async void OnTapGestureRecogniserTappedSecurityCamera(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }

    public async void OnTapGestureRecogniserTappedMovementSensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }

    public async void OnTapGestureRecogniserTappedWaterLeakSensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }

    public async void OnTapGestureRecogniserTappedHumiditySensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }

    public async void OnTapGestureRecogniserTappedVibrationSensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("devicedetails");
    }
}