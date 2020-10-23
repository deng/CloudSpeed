using System;

namespace CloudSpeed.Web.Models
{
    public class CloudSpeedItem
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string DataCid { get; set; }

        public string WxpayKey { get; set; }

        public string AlipayKey { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string MimeType { get; set; }

        public bool Secret { get; set; }

        public DateTime Created { get; set; }
    }
}
