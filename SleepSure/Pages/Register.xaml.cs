using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Register : ContentPage
{
	public Register(AuthenticationViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}
}