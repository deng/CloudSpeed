using System.IO;

namespace CloudSpeed.Settings
{
    public class UploadSetting
    {
        public string[] Storages { get; set; }

        public string RewardPath { get; set; }

        public long MinFileSize { get; set; }
        
        public long MaxFileSize { get; set; }

        public string GetStoragePath(string key)
        {
            if (Storages.Length == 0) return string.Empty;
            for (int i = 0; i < Storages.Length; i++)
            {
                var fullPath = Path.Combine(Storages[i], key);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return string.Empty;
        }

        public string GetRewardPath(string key)
        {
            var fullPath = Path.Combine(RewardPath, key);
            return fullPath;
        }
    }
}
