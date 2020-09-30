using System.Collections.Generic;
namespace CloudSpeed.Sdk
{
    public class LotusConstants
    {
        public const int EpochDurationSeconds = 30;

        public const int SecondsInHour = 60 * 60;

        public const int SecondsInDay = 24 * SecondsInHour;

        public const int EpochsInHour = SecondsInHour / EpochDurationSeconds;

        public const int EpochsInDay = SecondsInDay / EpochDurationSeconds;
    }

    public class SectorSizeConstants
    {
        public const long Bytes8MiB = 8388608;

        public const long Bytes512MiB = 536870912;

        public const long Bytes32GiB = 34359738368;

        public const long Bytes64GiB = 68719476736;
    }

    public class UploadConstants
    {
        public const int BigFileWriteSize = 84975;
    }
}