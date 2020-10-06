namespace CloudSpeed.Settings
{
    public class UploadSetting
    {
        public string[] Storages { get; set; }

        public string RewardPath { get; set; }

        public long MaxFileSize { get; set; }

        public int LimitUploading { get; set; }
    }
}
