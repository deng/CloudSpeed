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
using System.Linq;

namespace CloudSpeed.AdminWeb.Managers
{
    public class DealsManager
    {
        public DealsManager()
        {

        }

        public async Task<ApiResponse<IDictionary<FileDealStatus, int>>> GetDashboardInfo(DashboardDealsRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var counts = await repository.CountDealsGroupByStatus();
                return ApiResponse.Ok(counts);
            }
        }

        public async Task<ApiResponse<PagedResult<DealsListItem>>> GetDeals(DealsGetListRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var total = await repository.CountFileDeals();
                var entities = await repository.GetFileDeals(request.Skip, request.Limit);
                var result = entities.Select(entity => new DealsListItem
                {
                    Id = entity.Id,
                    Cid = entity.Cid,
                    DealId = entity.DealId,
                    Miner = entity.Miner,
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
