using Newtonsoft.Json;
using System;

namespace CloudSpeed.Sdk
{
    public class ClientStartDealRequest
    {
        public string DataCid { get; set; }

        public string Miner { get; set; }

        public string Price { get; set; }

        public long Duration { get; set; }

        public decimal AskingPrice { get; set; }

        public void CalPriceByDealSize(long dealSize)
        {
            var askingPrice = (long)((AskingPrice * dealSize) / (1 << 30)) + 1;
            Price = askingPrice.ToString();
        }
    }

    public class ClientStartDealParams
    {
        public TransferDataRef Data { get; set; }

        public string Wallet { get; set; }

        public string Miner { get; set; }

        public string EpochPrice { get; set; }

        public ulong MinBlocksDuration { get; set; }

        public string ProviderCollateral { get; set; }

        public long DealStartEpoch { get; set; }

        public bool FastRetrieval { get; set; }

        public bool VerifiedDeal { get; set; }
    }

    public class TransferDataRef
    {
        public string TransferType { get; set; }

        public Cid Root { get; set; }

        public Cid PieceCid { get; set; }

        public long PieceSize { get; set; }
    }
}
