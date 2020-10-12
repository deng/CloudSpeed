using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Entities;

namespace CloudSpeed.AdminWeb.Responses
{
    public class ImportsListItem
    {
        public string Id { get; set; }
        
        public string Path { get; set; }

        public FileImportStatus Status { get; set; }

        public long Total{ get; set; }

        public long Success { get; set; }

        public long Failed { get; set; }

        public string Error { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
