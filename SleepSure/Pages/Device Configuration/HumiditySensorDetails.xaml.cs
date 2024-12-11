using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class HumiditySensorDetails : ContentPage
{
    public HumiditySensorDetails(HumiditySensorDetailsViewModel viewmodel)
    {
        InitializeComponent();

        BindingContext = viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(Shell.Current.CurrentPage, false);

        if (BindingContext is HumiditySensorDetailsViewModel viewModel)
        {
            viewModel.RetrieveLocationsCommand.Execute(null);
        }
    }
}