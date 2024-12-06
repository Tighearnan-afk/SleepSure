using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class AddDevice : ContentPage
{
	public AddDevice(AddDeviceViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is AddDeviceViewModel viewModel)
        {
            viewModel.RetrieveDeviceTypesCommand.Execute(null);
        }
    }
}