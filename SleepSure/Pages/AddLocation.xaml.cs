namespace SleepSure.Pages;

public partial class AddLocation : ContentPage
{
	public AddLocation()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage,false);
    }
}