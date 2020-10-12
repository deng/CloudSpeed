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

namespace CloudSpeed.AdminWeb.Managers
{
    public class JobsManager
    {
        public JobsManager()
        {

        }

        public async Task<ApiResponse<IDictionary<FileJobStatus, int>>> GetDashboardInfo(DashboardJobsRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var counts = await repository.CountJobsGroupByStatus();
                return ApiResponse.Ok(counts);
            }
        }

        public async Task<ApiResponse<PagedResult<JobsListItem>>> GetJobs(JobsGetListRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var total = await repository.CountFileJobs();
                var entities = await repository.GetFileJobs(request.Skip, request.Limit);
                var result = entities.Select(entity => new JobsListItem
                {
                    Id = entity.Id,
                    Cid = entity.Cid,
                    JobId = entity.JobId,
                    Status = entity.Status,
                    Error = entity.Error,
                    Created = entity.Created,
                    Updated = entity.Updated,
                }).ToList().AsEnumerable();
                return ApiResponse.Ok(request.ToPagedResult(result, total));
            }
        }
    }
}
