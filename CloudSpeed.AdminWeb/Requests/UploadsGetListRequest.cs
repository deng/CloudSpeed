using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Web.Requests;
using CloudSpeed.Entities.DTO;
using CloudSpeed.Entities;
using CloudSpeed.Services;
using CloudSpeed.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CloudSpeed.AdminWeb.Requests
{
    public class UploadsGetListRequest : GetListRequest
    {
        public UploadsGetListRequestParamMap ParamMap { get; set; }

        public async Task<UploadParamMap> ToParamMapDTO()
        {
            if (ParamMap == null) return new UploadParamMap();

            var paramMap = new UploadParamMap();
            var memberManager = GlobalServices.ServiceProvider.GetService<UserManager<Member>>();
            if (!string.IsNullOrEmpty(ParamMap.UserName))
            {
                var member = await memberManager.FindByNameAsync(ParamMap.UserName);
                if (member != null)
                {
                    paramMap.UserId = member.Id;
                }
                else
                {
                    paramMap.UserId = System.Guid.NewGuid().ToString();
                }
            }
            return paramMap;
        }
    }

    public class UploadsGetListRequestParamMap
    {
        public string UserName { get; set; }
    }
}
