using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Dashboard : ContentPage
{
	public Dashboard(DeviceViewModel viewModel)
	{
		InitializeComponent();
        btnAddDevice.Clicked += OnBtnAddDevice_Clicked;

        //Set the binding context to the view model
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DeviceViewModel viewModel)
        {
            viewModel.GetDevicesCommand.Execute(null);
        }
    }

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