
namespace SleepSure.Pages;

public partial class Security : ContentPage
{
	public Security()
	{
		InitializeComponent();
	}

    private async void OnTapGestureRecogniserTappedBackDoorCamera(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("videofeed");
    }

    private async void OnTapGestureRecogniserTappedBackWallCamera(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("videofeed");
    }
}