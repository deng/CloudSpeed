using System.IO;

namespace CloudSpeed.Settings
{
    public class UploadSetting
    {
        public string[] Storages { get; set; }

        public string RewardPath { get; set; }

        public long MaxFileSize { get; set; }

        public int LimitUploading { get; set; }

        public string GetStoragePath(string key)
        {
            if (Storages.Length == 0) return string.Empty;
            var fullPath = Path.Combine(Storages[0], key);
            return fullPath;
        }

        public string GetRewardPath(string key)
        {
            var fullPath = Path.Combine(RewardPath, key);
            return fullPath;
        }
    }
}
