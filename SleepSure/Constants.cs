namespace SleepSure
{
    public class Constants
    {
        //Checks if the system is android and uses the 10.0.2.2 port if true otherwise uses localhost
        public static string LocalHostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string Scheme = "http";
        public static string Port = "5012";
        public static string RestUrl = $"{Scheme}://{LocalHostUrl}:{Port}/api/cameras/{{0}}";
    }
}
