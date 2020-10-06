using System;

namespace CloudSpeed.Sdk
{
    /*
{
    "jsonrpc": "2.0",
    "result": {
        "ProposalCid": {
            "/": "bafyreiggk2lvwqdvewfb7alqks4t3vyf2stpuzorkgf57dnmdtr2wgquly"
        },
        "State": 23,
        "Message": "",
        "Provider": "t01000",
        "DataRef": null,
        "PieceCID": {
            "/": "baga6ea4seaqekhvibptpost5xmnsjg4tshilqrspdesz6frhbwwyzqk65w6d6my"
        },
        "Size": 65024,
        "PricePerEpoch": "5000000",
        "Duration": 703365,
        "DealID": 0,
        "CreationTime": "2020-10-06T17:55:10.377814615+08:00"
    },
    "id": 0
}
    */
    public class ClientGetDealInfoReponse
    {
        public Cid ProposalCid { get; set; }

        public StorageDealStatus State { get; set; }

        public string Message { get; set; }

        public string Provider { get; set; }

        public TransferDataRef DataRef { get; set; }

        public Cid PieceCID { get; set; }

        public ulong Size { get; set; }

        public string PricePerEpoch { get; set; }

        public ulong Duration { get; set; }

        public ulong DealID { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
