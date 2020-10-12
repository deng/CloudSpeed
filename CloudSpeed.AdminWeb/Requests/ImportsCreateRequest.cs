using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CloudSpeed.AdminWeb.Requests
{
    public class ImportsCreateRequest
    {
        [Required]
        public string StoragePath { get; set; }
    }
}
