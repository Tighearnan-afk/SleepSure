using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Model
{
    [Table("video")]
    public class Video
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string VideoPath { get; set; }
        public int CameraId { get; set; }

        public Video() { }

        public Video(int cameraId)
        {
            CameraId = cameraId;
        }
    }
}
