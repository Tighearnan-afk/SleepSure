using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SleepSure.Model;
using SleepSure.Pages;
using SleepSure.Services;
using System.Diagnostics;

namespace SleepSure.ViewModel
{
    [QueryProperty("Camera", "Camera")]
    public partial class VideoFeedViewModel : BaseViewModel
    {
        //A service that allows videos to be recorded
        IVideoDataService _videoDataService;
        //Stores the camera object passed in from the security page
        [ObservableProperty]
        public Camera _camera;

        public VideoFeedViewModel(IVideoDataService videoDataservice)
        {
            _videoDataService = videoDataservice;
        }

        [RelayCommand]
        public async Task GoToVideoArchiveAsync()
        {
            if (Camera is null)
                return;

            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                //Navigate to the video archive page passing the selected camera object within a dictionary and a true value for animate
                await Shell.Current.GoToAsync($"{nameof(VideoArchive)}", true,
                    new Dictionary<string, object> { { "Camera", Camera } });
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally {
                IsBusy = false;
            }
        }

        /// <summary>
        /// The RecordVideoAsync method similates the recording of a livestream by creating a new video and associating it with the current camera
        /// </summary>
        [RelayCommand]
        public async Task RecordVideoAsync()
        {
            if (Camera is null)
                return;

            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await _videoDataService.AddVideoAsync((int)Camera.Id);
                await Shell.Current.DisplayAlert("Stream Archived", $"Stream successfully archived for {Camera.Name}", "OK");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
