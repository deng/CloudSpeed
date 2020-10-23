using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Autofac;
using CloudSpeed.Entities;
using CloudSpeed.Services;
using CloudSpeed.Settings;
using CloudSpeed.Repositories;
using System.Threading.Tasks;
using CloudSpeed.Web.Responses;
using CloudSpeed.Web.Models;
using CloudSpeed.Web.Requests;
using CloudSpeed.Sdk;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Responses;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using CloudSpeed.Identity;

namespace CloudSpeed.AdminWeb.Managers
{
    public class DealsManager
    {
        private readonly UploadSetting _uploadSetting;
        private readonly UserManager<Member> _memberManager;

        public DealsManager(UploadSetting uploadSetting, UserManager<Member> memberManager)
        {
            _uploadSetting = uploadSetting;
            _memberManager = memberManager;
        }

        public IDictionary<string, int> GetDashboardInfo()
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var counts = repository.CountDealsGroupByStatus();
                return counts;
            }
        }

        public async Task<ApiResponse<PagedResult<DealsListItem>>> GetDeals(DealsGetListRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var paramMap = request.ToParamMapDTO();
                var total = await repository.CountFileDeals(paramMap);
                var entities = await repository.GetFileDeals(paramMap, request.Skip, request.Limit);
                var result = entities.Select(entity => new DealsListItem
                {
                    Id = entity.Id,
                    Cid = entity.Cid,
                    DealId = entity.DealId,
                    Miner = entity.Miner,
                    Status = entity.Status.ToString(),
                    Error = entity.Error,
                    Created = entity.Created,
                    Updated = entity.Updated,
                }).ToList().AsEnumerable();
                foreach (var item in result)
                {
                    var fileCid = await repository.GetFileCidsByCid(item.Cid);
                    var fileName = await repository.GetFileName(fileCid.Id);
                    var path = _uploadSetting.GetStoragePath(fileCid.Id);
                    var mimeType = fileName.GetMimeType();
                    item.FileName = fileName;
                    item.Format = mimeType;
                    item.FileSize = new FileInfo(path).ToFileSize();
                }
                return ApiResponse.Ok(request.ToPagedResult(result, total));
            }
        }
    }
}
