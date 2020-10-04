namespace CloudSpeed.Entities
{
    public enum FileJobStatus : byte
    {
        None = 0,

        Processing = 1,

        Success = 2,

        Failed = 3,

        Canceled = 4
    }
}
