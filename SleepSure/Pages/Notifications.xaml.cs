using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class Notifications : ContentPage
{
    public Notifications(NotificationViewModel viewModel)
    {
        InitializeComponent();

        //Set the binding context to the view model
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is NotificationViewModel viewModel)
        {
            viewModel.GetAlarmsCommand.Execute(null);
        }
    }
}