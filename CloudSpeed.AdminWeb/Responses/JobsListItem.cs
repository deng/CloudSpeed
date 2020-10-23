using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Entities;

namespace CloudSpeed.AdminWeb.Responses
{
    public class JobsListItem
    {
        public string Id { get; set; }
        
        public string Cid { get; set; }

        public string JobId { get; set; }

        public string Status { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public string Format { get; set; }

        public string Error { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
