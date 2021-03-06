using System;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;
using CloudSpeed.Services;

namespace CloudSpeed.Sdk
{
    public class LotusClientSetting
    {
        public LotusApi Api { get; set; }

        public LotusMinerSetting[] Miners { get; set; }

        public string MaxTransferingSize { get; set; }

        public int MaxTransferingCount { get; set; }

        public bool Enabled { get; set; }

        public LotusMinerSetting GetMinerByFileSize(long size, bool online)
        {
            if (Miners == null || Miners.Length == 0)
                return null;
            var sectorSize = string.Empty;
            var filterMiner = Miners.Where(m => m.Online == online).ToArray();
            if (filterMiner.Length == 0) return null;
            var orderedMiners = filterMiner.Where(a => a.SectorSizeInBytes >= size).OrderBy(m => m.SectorSizeInBytes).ToArray();
            if (orderedMiners.Length > 0)
                return orderedMiners[RandomService.Next(0, orderedMiners.Length)];
            else
                return filterMiner[RandomService.Next(0, filterMiner.Length)];
        }
    }

    public class LotusMinerSetting
    {
        public string Miner { get; set; }

        public string SectorSize { get; set; }

        public decimal AskingPrice { get; set; }

        public LotusApi Api { get; set; }

        public bool Online { get; set; }

        public long SectorSizeInBytes
        {
            get
            {
                return GetMaxTransferingSizeInBytes(SectorSize);
            }
        }

        public static long GetMaxTransferingSizeInBytes(string sectorSize)
        {
            switch (sectorSize)
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
