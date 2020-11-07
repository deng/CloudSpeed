using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
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
using System.Linq;
using CloudSpeed.Entities.DTO;

namespace CloudSpeed.Managers
{
    public class CloudSpeedManager
    {
        private readonly ILogger _logger;
        private readonly IPanPasswordHasher _panPasswordHasher;
        private readonly UploadSetting _uploadSetting;
        private readonly LotusClient _lotusClient;

        public CloudSpeedManager(ILogger<CloudSpeedManager> logger,
            IPanPasswordHasher panPasswordHasher, UploadSetting uploadSetting, LotusClient lotusClient)
        {
            _logger = logger;
            _panPasswordHasher = panPasswordHasher;
            _uploadSetting = uploadSetting;
            _lotusClient = lotusClient;
        }

        public async Task<ApiResponse<string>> CreateUploadLog(string userId, PanePostRequest request)
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
                UserId = userId
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
                var fileFullPath = _uploadSetting.GetStoragePath(uploadLog.DataKey);
                if (File.Exists(fileFullPath))
                {
                    item.FileSize = new FileInfo(fileFullPath).Length;
                    item.MimeType = fileName.GetMimeType();
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

                var path = _uploadSetting.GetStoragePath(uploadLog.DataKey);
                var fileName = await GetFileName(uploadLog.DataKey);
                var mimeType = fileName.GetMimeType();

                return ApiResponse.Ok(new FilDataInfo { Path = path, FileName = fileName, MimeType = mimeType });
            }
        }

        public async Task<ApiResponse<PagedResult<MyFileInfo>>> GetMyFiles(string userId, MyFilesPostRequest request)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var paramMap = new UploadParamMap() { UserId = userId };
                var uploadLogs = await repository.GetUploadLogs(paramMap, request.Skip, request.Limit);
                var total = await repository.CountUploadLogs(paramMap);

                var myFiles = new List<MyFileInfo>();
                foreach (var uploadLog in uploadLogs)
                {
                    var path = _uploadSetting.GetStoragePath(uploadLog.DataKey);
                    var fileName = await GetFileName(uploadLog.DataKey);
                    var mimeType = fileName.GetMimeType();
                    var fileInfo = new FileInfo(path);
                    var fileSize = (fileInfo.Exists ? fileInfo.Length : 0).ToFileSize();
                    var fileCid = await repository.GetFileCid(uploadLog.DataKey);
                    myFiles.Add(new MyFileInfo
                    {
                        FileName = fileName,
                        FileSize = fileSize,
                        Id = uploadLog.Id,
                        DataCid = fileCid?.Cid,
                        Created = uploadLog.Created,
                        Format = mimeType
                    });
                }
                return ApiResponse.Ok(request.ToPagedResult(myFiles, total));
            }
        }

        public async Task<ApiResponse<string>> CreateFileName(string id, string name, long size)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileName()
                {
                    Id = id,
                    Name = name,
                    Size = size,
                    Format = name.GetMimeType()
                };
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

        public async Task<FileCid> GetFileCidByCid(string cid)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var fileCid = await repository.GetFileCidsByCid(cid);
                return fileCid;
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

        public async Task UpdateFileCidDealSize(string id, long dealSize, long payloadSize)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileCidDealSize(id, dealSize, payloadSize);
                await repository.Commit();
            }
        }

        public async Task UpdateFileCid(string id, FileCidStatus status, string error = "")
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileCid(id, status, error);
                await repository.Commit();
            }
        }

        public async Task<IList<FileJob>> GetFileJobs(FileJobStatus status, int skip, int limit)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var fileJobs = await repository.GetFileJobs(status, skip, limit);
                return fileJobs;
            }
        }

        public async Task<string> CreateFileJob(string cid)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileJob()
                {
                    Cid = cid
                };
                await repository.CreateFileJob(entity);
                await repository.Commit();
                return entity.Id;
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

        public async Task UpdateFileJob(string id, FileJobStatus status, string error = "")
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileJob(id, status, error);
                await repository.Commit();
            }
        }

        public async Task<IList<FileDeal>> GetFileDeals(FileDealStatus status, int skip, int limit)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var fileJobs = await repository.GetFileDeals(status, skip, limit);
                return fileJobs;
            }
        }

        public async Task<string> CreateFileDeal(string cid, string pieceCid, long pieceSize)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileDeal()
                {
                    Cid = cid,
                    PieceCid = pieceCid,
                    PieceSize = pieceSize
                };
                await repository.CreateFileDeal(entity);
                await repository.Commit();
                return entity.Id;
            }
        }

        public async Task UpdateFileDeal(string id, string pieceCid, long pieceSize)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileDeal(id, pieceCid, pieceSize);
                await repository.Commit();
            }
        }

        public async Task UpdateFileDeal(string id, string miner, string dealId, FileDealStatus status)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileDeal(id, miner, dealId, status);
                await repository.Commit();
            }
        }

        public async Task UpdateFileDeal(string id, FileDealStatus status, string error = "")
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                await repository.UpdateFileDeal(id, status, error);
                await repository.Commit();
            }
        }

        public async Task<bool> CheckFileMd5ById(string id)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var hasEntity = await repository.HasFileMd5(id);
                return hasEntity;
            }
        }

        public async Task<ApiResponse<FileMd5>> GetFileMd5ByDataKey(string key)
        {
            var path = _uploadSetting.GetStoragePath(key);
            if (!File.Exists(path))
                return ApiResponse.NotFound<FileMd5>("file not exists");
            var md5 = GetMD5HashFromFile(path);
            if (string.IsNullOrEmpty(md5))
                return ApiResponse.NotFound<FileMd5>("file md5 failed");

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

        public async Task<ApiResponse<string>> CreateFileMd5ByDataKey(string key)
        {
            var path = _uploadSetting.GetStoragePath(key);
            if (!File.Exists(path))
                return ApiResponse.NotFound<string>("file not exists");
            var md5 = GetMD5HashFromFile(path);
            if (string.IsNullOrEmpty(md5))
                return ApiResponse.NotFound<string>("file md5 failed");
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileMd5() { Id = md5, DataKey = key };
                await repository.CreateFileMd5(entity);
                await repository.Commit();
                return ApiResponse.Ok<string>(entity.Id);
            }
        }

        public async Task CreateFileMd5(string id, string dataKey)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var entity = new FileMd5() { Id = id, DataKey = dataKey };
                await repository.CreateFileMd5(entity);
                await repository.Commit();
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
            catch
            {
                return string.Empty;
            }
        }

        public async Task<ApiResponse<DownloadingFileInfo>> GetFileByCid(string cid)
        {
            using (var scope = GlobalServices.Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<ICloudSpeedRepository>();
                var fileCid = await repository.GetFileCidsByCid(cid);
                var downloadingFileInfo = new DownloadingFileInfo();
                if (fileCid != null)
                {
                    var path = _uploadSetting.GetStoragePath(fileCid.Id);
                    var fileName = await GetFileName(fileCid.Id);
                    var mimeType = fileName.GetMimeType();
                    var fileDeal = await repository.GetFileDealByCid(cid);
                    var uploadLog = await repository.GetUploadLogByDataKey(fileCid.Id);
                    var lsInfo = new LocalStroeInfo()
                    {
                        MimeType = mimeType,
                        FileName = fileName,
                        FileSize = new FileInfo(path).ToFileSize(),
                        Date = fileCid.Created,
                        Publisher = "ming",
                        Miner = fileDeal != null ? (fileDeal.Miner + "-" + fileDeal.Status) : string.Empty,
                        LogId = uploadLog.Id
                    };
                    downloadingFileInfo.LocalStroeInfo = lsInfo;
                    if (fileDeal != null && (fileDeal.Status == FileDealStatus.Success || fileDeal.Status == FileDealStatus.Processing))
                    {
                        var queryOffers = await _lotusClient.ClientFindData(new ClientFindDataRequest { Root = new Cid() { Value = cid } });
                        if (queryOffers.Success)
                        {
                            var orderInfos = new List<RetrievalOrderInfo>();
                            foreach (var item in queryOffers.Result)
                            {
                                if (orderInfos.Any(o => o.Miner == item.Miner))
                                    continue;
                                var orderInfo = new RetrievalOrderInfo()
                                {
                                    Miner = item.Miner,
                                    MinerPeerId = item.MinerPeer.ID,
                                    OfferMinPrice = item.MinPrice,
                                    OfferSize = ((long)item.Size).ToFileSize(),
                                    Err = item.Err
                                };
                                orderInfos.Add(orderInfo);
                            }
                            downloadingFileInfo.RetrievalOrderInfos = orderInfos.ToArray();
                        }
                        else
                        {
                            _logger.LogWarning(queryOffers.Error?.Message);
                        }
                    }
                }
                return ApiResponse.Ok(downloadingFileInfo);
            }
        }
    }
}
