namespace SleepSure.Pages;

public partial class Dashboard : ContentPage
{
	public Dashboard()
	{
		InitializeComponent();
	}

    public async void OnTapGestureRecogniserTappedLight(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("lightdevicedetails");
    }

    public async void OnTapGestureRecogniserTappedThermostat(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("thermostatdevicedetails");
    }

    public async void OnTapGestureRecogniserTappedSecurityCamera(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("thermostatdevicedetails");
    }

    public async void OnTapGestureRecogniserTappedMovementSensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("thermostatdevicedetails");
    }

    public async void OnTapGestureRecogniserTappedWaterLeakSensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("thermostatdevicedetails");
    }

    public async void OnTapGestureRecogniserTappedHumiditySensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("thermostatdevicedetails");
    }

    public async void OnTapGestureRecogniserTappedVibrationSensor(object? sender, TappedEventArgs args)
    {
        await Shell.Current.GoToAsync("thermostatdevicedetails");
    }
}