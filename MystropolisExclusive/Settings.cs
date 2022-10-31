using Windows.Storage;

namespace MystropolisExclusive
{
    public static class Settings
    {

        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static int CountdownDuration => (int?)localSettings.Values[Keys.CountdownDuration] ?? 10;

        public static string VideoFolderName => (string)localSettings.Values[Keys.VideoFolderName] ?? "Mystifik";

        public static class Keys
        {
            public const string CountdownDuration = "CountdownDuration";
            public const string VideoFolderName = "VideoFolderName";
        }
    }
}
