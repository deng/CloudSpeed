using System;

namespace CloudSpeed.Web.Models
{
    public class DownloadingFileInfo
    {
        public LocalStroeInfo LocalStroeInfo { get; set; }

        public RetrievalOrderInfo[] RetrievalOrderInfos { get; set; }
    }

    public class LocalStroeInfo
    {
        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime Date { get; set; }

        public string Publisher { get; set; }

        public string FileSize { get; set; }

        public string Miner { get; set; }

        public string LogId { get; set; }
    }

    public class RetrievalOrderInfo
    {
        public string Miner { get; set; }

        public string MinerPeerId { get; set; }

        public string OfferMinPrice { get; set; }

        public string OfferSize { get; set; }

        public string Err { get; set; }
    }
}
