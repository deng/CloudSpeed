using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Web.Requests;
using CloudSpeed.Entities;
using CloudSpeed.Entities.DTO;

namespace CloudSpeed.AdminWeb.Requests
{
    public class DealsGetListRequest : GetListRequest
    {
        public DealsGetListRequestParamMap ParamMap { get; set; }

        public FileDealParamMap ToParamMapDTO()
        {
            if (ParamMap == null) return new FileDealParamMap();

            return new FileDealParamMap()
            {
                Status = string.IsNullOrEmpty(ParamMap.Status) ? new Nullable<FileDealStatus>() : (FileDealStatus)Enum.Parse(typeof(FileDealStatus), ParamMap.Status)
            };
        }
    }

    public class DealsGetListRequestParamMap
    {
        public string Status { get; set; }
    }
}
