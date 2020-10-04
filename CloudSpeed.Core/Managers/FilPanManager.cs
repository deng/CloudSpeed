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

namespace CloudSpeed.Managers
{
    public class CloudSpeedManager
    {
        private readonly IPanPasswordHasher _panPasswordHasher;
        private readonly UploadSetting _uploadSetting;

        public CloudSpeedManager(IPanPasswordHasher panPasswordHasher, UploadSetting uploadSetting)
        {
            _panPasswordHasher = panPasswordHasher;
            _uploadSetting = uploadSetting;
        }

        public async Task<ApiResponse<string>> CreateUploadLog(PanePostRequest request)
        {
            var hashedPassword = string.Empty;
            if (!string.IsNullOrEmpty(request.Password))
            {
                hashedPassword = await _panPasswordHasher.HashPassword(request.Password);
            }
            var uploadlog = new UploadLog()
            {
                Description = request.Description,
                DataKey = request.DataKey,
                HashedPassword = hashedPassword,
                AlipayFileKey = request.AlipayKey,
                WxpayFileKey = request.WxpayKey,
            };
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.CreateUploadLog(uploadlog);
                if (!await repository.HasFileCid(request.DataKey))
                {
                    var fileCid = new FileCid()
                    {
                        Id = request.DataKey
                    };
                    await repository.CreateFileCid(fileCid);
                }
                await repository.Commit();
            }
            return ApiResponse.Ok(uploadlog.Id);
        }

        public async Task<ApiResponse<CloudSpeedItem>> GetUploadLog(string id)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var uploadLog = await repository.GetUploadLog(id);
                if (uploadLog == null)
                    return ApiResponse.NotFound<CloudSpeedItem>("item not found");
                var fileName = await repository.GetFileName(uploadLog.DataKey);
                var item = new CloudSpeedItem
                {
                    Id = id,
                    Description = uploadLog.Description,
                    AlipayKey = uploadLog.AlipayFileKey,
                    WxpayKey = uploadLog.WxpayFileKey,
                    Created = uploadLog.Created,
                    FileName = fileName,
                };
                var fileFullPath = GetStoragePath(uploadLog.DataKey);
                if (File.Exists(fileFullPath))
                {
                    item.FileSize = new FileInfo(fileFullPath).Length;
                }
                var fileCid = await repository.GetFileCid(uploadLog.DataKey);
                if (fileCid != null && fileCid.Status == FileCidStatus.Success)
                {
                    item.DataCid = fileCid.Cid;
                }
                if (!string.IsNullOrEmpty(uploadLog.HashedPassword))
                {
                    item.Secret = true;
                }
                return ApiResponse.Ok(item);
            }
        }

        public async Task<ApiResponse<bool>> ValidateUploadLog(string id, string password)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var uploadLog = await repository.GetUploadLog(id);
                if (uploadLog == null)
                    return ApiResponse.NotFound<bool>("invalid upload log id");
                if (string.IsNullOrEmpty(uploadLog.HashedPassword))
                    return ApiResponse.Ok(true);
                if (string.IsNullOrEmpty(password))
                    return ApiResponse.BadRequestResult<bool>("password required");
                var result = await _panPasswordHasher.VerifyHashedPassword(uploadLog.HashedPassword, password);
                return ApiResponse.Ok(result);
            }
        }

        public async Task<ApiResponse<FilDataInfo>> GetFilDataInfo(string id)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var uploadLog = await repository.GetUploadLog(id);
                if (uploadLog == null)
                    return ApiResponse.NotFound<FilDataInfo>("invalid upload log id");

                var path = GetStoragePath(uploadLog.DataKey);
                var fileName = await GetFileName(uploadLog.DataKey);
                var mimeType = GetMimeType(fileName);

                return ApiResponse.Ok(new FilDataInfo { Path = path, FileName = fileName, MimeType = mimeType });
            }
        }

        public async Task<ApiResponse<string>> CreateFileName(string item1, string item2)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileName() { Id = item1, Name = item2 };
                await repository.CreateFileName(entity);
                await repository.Commit();
                return ApiResponse.Ok(entity.Id);
            }
        }

        public async Task<string> GetFileName(string key)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var fileName = await repository.GetFileName(key);
                return fileName;
            }
        }

        public async Task<IList<FileCid>> GetFileCids(FileCidStatus status, int skip, int limit)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var fileCids = await repository.GetFileCids(status, skip, limit);
                return fileCids;
            }
        }

        public async Task UpdateFileCid(string id, string cid, FileCidStatus status)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileCid(id, cid, status);
                await repository.Commit();
            }
        }

        public async Task CreateFileJob(string id, string cid)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileJob()
                {
                    Id = id,
                    Cid = cid
                };
                await repository.CreateFileJob(entity);
                await repository.Commit();
            }
        }

        public async Task UpdateFileJob(string id, string jobId, FileJobStatus status)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileJob(id, jobId, status);
                await repository.Commit();
            }
        }

        public string GetMimeType(string fileName)
        {
            return MimeTypes.GetMimeType(fileName);
        }

        public string GetStoragePath(string key)
        {
            var fullPath = Path.Combine(_uploadSetting.Storages[0], key);
            return fullPath;
        }

        public string GetRewardPath(string key)
        {
            var fullPath = Path.Combine(_uploadSetting.RewardPath, key);
            return fullPath;
        }

        public async Task<ApiResponse<FileMd5>> GetFileMd5(string key)
        {
            var path = GetStoragePath(key);
            if (!File.Exists(path))
                return ApiResponse.NotFound<FileMd5>("file not exists");
            var md5 = GetMD5HashFromFile(path);
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var md5Entity = await repository.GetFileMd5(md5);
                if (md5Entity != null)
                    return ApiResponse.Ok<FileMd5>(md5Entity);
                else
                    return ApiResponse.NotFound<FileMd5>("md5Entity not exists");
            }
        }

        public async Task<ApiResponse<string>> CreateFileMd5(string key)
        {
            var path = GetStoragePath(key);
            if (!File.Exists(path))
                return ApiResponse.NotFound<string>("file not exists");
            var md5 = GetMD5HashFromFile(path);
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileMd5() { Id = md5, DataKey = key };
                await repository.CreateFileMd5(entity);
                await repository.Commit();
                return ApiResponse.Ok<string>(entity.Id);
            }
        }

        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                using (var file = new FileStream(fileName, FileMode.Open))
                {
                    using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                    {
                        var retVal = md5.ComputeHash(file);
                        var sb = new StringBuilder();
                        for (int i = 0; i < retVal.Length; i++)
                        {
                            sb.Append(retVal[i].ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
