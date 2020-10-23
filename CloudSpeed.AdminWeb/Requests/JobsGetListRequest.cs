using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Web.Requests;
using CloudSpeed.Entities.DTO;
using CloudSpeed.Entities;

namespace CloudSpeed.AdminWeb.Requests
{
    public class JobsGetListRequest : GetListRequest
    {
        public JobsGetListRequestParamMap ParamMap { get; set; }

        public FileJobParamMap ToParamMapDTO()
        {
            if (ParamMap == null) return new FileJobParamMap();

            return new FileJobParamMap()
            {
                Status = string.IsNullOrEmpty(ParamMap.Status) ? new Nullable<FileJobStatus>() : (FileJobStatus)Enum.Parse(typeof(FileJobStatus), ParamMap.Status)
            };
        }
    }

    public class JobsGetListRequestParamMap
    {
        public string Status { get; set; }
    }
}
