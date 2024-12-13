using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Profile : ContentPage
{
	public Profile(AuthenticationViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}
}