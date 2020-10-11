using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSpeed.AdminWeb.Requests
{
    public class GetListRequest
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;
    }
}
