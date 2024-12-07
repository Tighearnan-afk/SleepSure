using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class VideoFeed : ContentPage
{
	public VideoFeed(VideoFeedViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
    }
}