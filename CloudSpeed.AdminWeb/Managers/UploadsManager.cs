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
    public class UploadsManager
    {
        private readonly UploadSetting _uploadSetting;
        private readonly UserManager<Member> _memberManager;

        public UploadsManager(UploadSetting uploadSetting, UserManager<Member> memberManager)
        {
            _uploadSetting = uploadSetting;
            _memberManager = memberManager;
        }

        public async Task<ApiResponse<PagedResult<UploadsListItem>>> GetUploads(UploadsGetListRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var paramMap = await request.ToParamMapDTO();
                var total = await repository.CountUploadLogs(paramMap);
                var entities = await repository.GetUploadLogs(paramMap, request.Skip, request.Limit);
                var uploadLogs = entities.ToList();
                var result = new List<UploadsListItem>();
                foreach (var uploadLog in uploadLogs)
                {
                    var item = new UploadsListItem
                    {
                        Id = uploadLog.Id,
                        Created = uploadLog.Created,
                    };
                    var fileCid = await repository.GetFileCid(uploadLog.DataKey);
                    var fileName = await repository.GetFileName(fileCid.Id);
                    var path = _uploadSetting.GetStoragePath(fileCid.Id);
                    var member = await _memberManager.FindByIdAsync(uploadLog.UserId);
                    var mimeType = fileName.GetMimeType();
                    item.Cid = fileCid.Cid;
                    item.UserName = member?.UserName;
                    item.FileName = fileName;
                    item.Format = mimeType;
                    item.FileSize = new FileInfo(path).ToFileSize();
                    result.Add(item);
                }
                return ApiResponse.Ok(request.ToPagedResult(result, total));
            }
        }
    }
}
