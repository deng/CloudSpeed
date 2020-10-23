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
using CloudSpeed.Web.Models;
using CloudSpeed.Web.Requests;
using CloudSpeed.Web.Responses;
using CloudSpeed.Sdk;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Responses;
using System.Linq;
using CloudSpeed.Identity;
using Microsoft.AspNetCore.Identity;

namespace CloudSpeed.AdminWeb.Managers
{
    public class JobsManager
    {
        private readonly UploadSetting _uploadSetting;
        private readonly UserManager<Member> _memberManager;

        public JobsManager(UploadSetting uploadSetting, UserManager<Member> memberManager)
        {
            _uploadSetting = uploadSetting;
            _memberManager = memberManager;
        }

        public IDictionary<string, int> GetDashboardInfo()
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var counts = repository.CountJobsGroupByStatus();
                return counts;
            }
        }

        public async Task<ApiResponse<PagedResult<JobsListItem>>> GetJobs(JobsGetListRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var paramMap = request.ToParamMapDTO();
                var total = await repository.CountFileJobs(paramMap);
                var entities = await repository.GetFileJobs(paramMap, request.Skip, request.Limit);
                var result = entities.Select(entity => new JobsListItem
                {
                    Id = entity.Id,
                    Cid = entity.Cid,
                    JobId = entity.JobId,
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
