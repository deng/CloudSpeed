using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Entities;

namespace CloudSpeed.AdminWeb.Responses
{
    public class DashboardInfo
    {
        public IDictionary<string, int> Jobs { get; set; }
        
        public IDictionary<string, int> Deals { get; set; }
    }
}
