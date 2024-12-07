using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class UpdateLocation : ContentPage
{
	public UpdateLocation(LocationViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);
    }
}