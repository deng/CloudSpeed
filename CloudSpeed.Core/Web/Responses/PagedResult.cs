using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSpeed.Web.Responses
{
    public class PagedResult<T>
    {
        public IEnumerable<T> List { get; set; }

        public int Total { get; set; }

        public int TotalPage { get; set; }

        public int Skip { get; set; }

        public int Limit { get; set; }
    }

    public class PagedResult<T, Data> : PagedResult<T>
    {
        public Data ExtraData { get; set; }
    }
}