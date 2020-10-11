using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSpeed.AdminWeb.Responses
{
    public class JobsListItem
    {
        public string Id { get; set; }
        
        public string Cid { get; set; }

        public string JobId { get; set; }

        public FileJobStatus Status { get; set; }

        public string Error { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
