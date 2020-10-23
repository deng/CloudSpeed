using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Web.Responses;

namespace CloudSpeed.Web.Requests
{
    public class GetListRequest
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;
    }

     public static class GetListRequestExtensions
    {
        public static PagedResult<T> ToPagedResult<T>(this GetListRequest request, IEnumerable<T> dataList, int total)
        {
            var totalPage = total == 0 ? 0 : (int)Math.Ceiling((double)total / request.Limit);
            return new PagedResult<T>()
            {
                List = dataList,
                TotalPage = totalPage,
                Total = total,
                Limit = request.Limit,
                Skip = request.Skip
            };
        }

        public static PagedResult<T, Data> ToPagedResult<T, Data>(this GetListRequest request, IEnumerable<T> dataList, int total, Data extraData)
        {
            var totalPage = total == 0 ? 0 : (int)Math.Ceiling((double)total / request.Limit);
            return new PagedResult<T, Data>()
            {
                List = dataList,
                TotalPage = totalPage,
                Total = total,
                Limit = request.Limit,
                Skip = request.Skip,
                ExtraData = extraData
            };
        }
    }
}
