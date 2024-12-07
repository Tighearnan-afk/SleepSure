using SleepSure.ViewModel;

namespace SleepSure.Pages;

public partial class VideoArchive : ContentPage
{
	public VideoArchive(VideoArchiveViewModel viewmodel)
	{
		InitializeComponent();

        BindingContext = viewmodel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is VideoArchiveViewModel viewModel)
        {
            viewModel.GetVideosCommand.Execute(null);
        }
    }
}