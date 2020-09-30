using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public class QueryOffer
    {
        public string Err { get; set; }

        public Cid Root { get; set; }

        public Cid Piece { get; set; }

        public ulong Size { get; set; }

        public string MinPrice { get; set; }

        public string UnsealPrice { get; set; }

        public ulong PaymentInterval { get; set; }

        public ulong PaymentIntervalIncrease { get; set; }

        public string Miner { get; set; }

        public RetrievalPeer MinerPeer { get; set; }
    }

    public class RetrievalPeer
    {
        public string Address { get; set; }

        public string ID { get; set; }

        public Cid PieceCID { get; set; }
    }
}
