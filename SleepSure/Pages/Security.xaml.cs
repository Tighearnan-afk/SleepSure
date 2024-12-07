
using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Security : ContentPage
{
	public Security(SecurityViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        if (BindingContext is SecurityViewModel viewModel)
        {
            viewModel.GetCamerasCommand.Execute(null);
        }
    }
}