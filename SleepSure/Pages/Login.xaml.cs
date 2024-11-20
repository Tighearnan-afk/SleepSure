using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Login : ContentPage
{
	public Login(AuthenticationViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
    }
}