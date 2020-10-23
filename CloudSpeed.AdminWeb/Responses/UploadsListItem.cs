using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Entities;

namespace CloudSpeed.AdminWeb.Responses
{
    public class UploadsListItem
    {
        public string Id { get; set; }

        public string Cid { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public string Format { get; set; }
        
        public string UserName { get; set; }

        public DateTime Created { get; set; }
   }
}
