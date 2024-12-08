using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class VideoFeed : ContentPage
{
	public VideoFeed(VideoFeedViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Stream.Play();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //Media element must be pauses instead of stopped due to stopping the Media element causing crashes on android
        Stream.Pause();
    }
}