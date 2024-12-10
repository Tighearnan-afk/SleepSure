using CommunityToolkit.Maui.Views;
using SQLite;

namespace SleepSure.Model
{
    [Table("video")]
    public class Video
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string VideoPath { get; set; }
        [Ignore]
        public MediaSource? MediaSource { get; set; }
        public int CameraId { get; set; }

        public Video() { }

        public Video(int cameraId)
        {
            CameraId = cameraId;
            VideoPath = "motion_detected.mp4";
        }
    }
}
