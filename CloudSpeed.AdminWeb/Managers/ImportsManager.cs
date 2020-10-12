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
    public class ImportsManager
    {
        public ImportsManager()
        {

        }

        public async Task<ApiResponse<PagedResult<ImportsListItem>>> GetImports(ImportsGetListRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var total = await repository.CountFileImports();
                var entities = await repository.GetFileImports(request.Skip, request.Limit);
                var result = entities.Select(entity => new ImportsListItem
                {
                    Id = entity.Id,
                    Path = entity.Path,
                    Status = entity.Status,
                    Total = entity.Total,
                    Success = entity.Success,
                    Failed = entity.Failed,
                    Error = entity.Error,
                    Created = entity.Created,
                    Updated = entity.Updated,
                }).ToList().AsEnumerable();
                return ApiResponse.Ok(request.ToPagedResult(result, total));
            }
        }

        public async Task<string> CreateFileImport(string path)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileImport()
                {
                    Path = path
                };
                await repository.CreateFileImport(entity);
                await repository.Commit();
                return entity.Id;
            }
        }
    }
}
