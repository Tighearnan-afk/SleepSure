using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Pages;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("Camera", "Camera")]
    public partial class VideoFeedViewModel : BaseViewModel
    {
        //Stores the camera object passed in from the security page
        [ObservableProperty]
        public Camera _camera;

        public VideoFeedViewModel()
        {
     
        }

        [RelayCommand]
        public async Task GoToVideoArchiveAsync()
        {
            if (Camera is null)
                return;

            try
            {
                //Navigate to the video archive page passing the selected camera object within a dictionary and a true value for animate
                await Shell.Current.GoToAsync($"{nameof(VideoArchive)}", true,
                    new Dictionary<string, object> { { "Camera", Camera } });
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
