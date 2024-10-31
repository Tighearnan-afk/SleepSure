using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SleepSure.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(_isNotBusy))]
        bool _isBusy;
        [ObservableProperty]
        string _pageTitle;

        bool _isNotBusy => !_isBusy;
    }
}
