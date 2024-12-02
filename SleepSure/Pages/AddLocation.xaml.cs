using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class AddLocation : ContentPage
{
	public AddLocation(AddLocationViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage,false);
    }
}