using System;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;

namespace CloudSpeed.Sdk
{
    public class LotusClientSetting
    {
        public string LotusApi { get; set; }

        public string LotusToken { get; set; }

        public int LotusTimeout { get; set; } = -1;

        public LotusMinerSetting[] Miners { get; set; }

        public bool Enabled { get; set; }

        public string GetMinerByFileSize(long size)
        {
            if (Miners == null || Miners.Length == 0)
                return string.Empty;
            var sectorSize = string.Empty;
            var orderedMiners = Miners.Where(a => a.SectorSizeInBytes >= size).OrderBy(m => m.SectorSizeInBytes).ToArray();
            if (orderedMiners.Length > 0)
                return orderedMiners.FirstOrDefault().Miner;
            else
                return Miners.OrderByDescending(m => m.SectorSizeInBytes).First().Miner;
        }
    }

    public class LotusMinerSetting
    {
        public string Miner { get; set; }

        public string SectorSize { get; set; }

        public long SectorSizeInBytes
        {
            get
            {
                switch (SectorSize)
                {
                    case "8MiB":
                        return SectorSizeConstants.Bytes8MiB;
                    case "512MiB":
                        return SectorSizeConstants.Bytes512MiB;
                    case "32GiB":
                        return SectorSizeConstants.Bytes32GiB;
                    case "64GiB":
                        return SectorSizeConstants.Bytes64GiB;
                    default:
                        return 0;
                }
            }
        }
    }
}
