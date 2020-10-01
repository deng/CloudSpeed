using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public class StorageAsk
    {
        public string Price { get; set; }

        public string VerifiedPrice { get; set; }

        public ulong MinPieceSize { get; set; }

        public ulong MaxPieceSize { get; set; }

        public string Miner { get; set; }

        public long Timestamp { get; set; }

        public long Expiry { get; set; }

        public ulong SeqNo { get; set; }
    }
}
