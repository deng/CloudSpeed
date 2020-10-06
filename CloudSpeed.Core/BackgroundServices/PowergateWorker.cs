using CloudSpeed.Sdk;
using CloudSpeed.Entities;
using CloudSpeed.Managers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using CloudSpeed.Powergate;
using Google.Protobuf;
using CloudSpeed.Services;

namespace CloudSpeed.BackgroundServices
{
    public class PowergateWorker : BackgroundService
    {
        private readonly ILogger<PowergateWorker> _logger;

        private readonly CloudSpeedManager _cloudSpeedManager;

        public PowergateWorker(ILogger<PowergateWorker> logger, CloudSpeedManager cloudSpeedManager)
        {
            _logger = logger;
            _cloudSpeedManager = cloudSpeedManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PowergateWorker running at: {time}", DateTimeOffset.Now);
            var powergateClient = GlobalServices.ServiceProvider.GetService<PowergateClient>();
            try
            {
                var ffsInfo = await powergateClient.Ffs.InfoAsync(new Ffs.Rpc.InfoRequest(), powergateClient.BotXFfsToken);
                _logger.LogInformation("Ffs info: {info}", Newtonsoft.Json.JsonConvert.SerializeObject(ffsInfo.Info));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(0, ex, "powergate ffs info fail:" + ex.ToString());
            }
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    {
                        int limit = 10;
                        int skip = 0;
                        while (true)
                        {
                            var fileCids = await _cloudSpeedManager.GetFileCids(FileCidStatus.None, skip, limit);
                            if (fileCids.Count() == 0)
                            {
                                break;
                            }
                            foreach (var fileCid in fileCids)
                            {
                                var path = _cloudSpeedManager.GetStoragePath(fileCid.Id);
                                if (File.Exists(path))
                                {
                                    var cid = string.Empty;
                                    try
                                    {
                                        using (var stage = powergateClient.Ffs.Stage(powergateClient.BotXFfsToken, cancellationToken: stoppingToken))
                                        {
                                            using (var fsRead = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, UploadConstants.BigFileWriteSize, true))
                                            {
                                                var buffer = new byte[UploadConstants.BigFileWriteSize];
                                                var readCount = 0;
                                                while ((readCount = await fsRead.ReadAsync(buffer, 0, buffer.Length, cancellationToken: stoppingToken)) > 0)
                                                {
                                                    await stage.RequestStream.WriteAsync(new Ffs.Rpc.StageRequest()
                                                    {
                                                        Chunk = ByteString.CopyFrom(buffer)
                                                    });
                                                }
                                            }
                                            await stage.RequestStream.CompleteAsync();
                                            var response = await stage.ResponseAsync;
                                            if (response != null)
                                            {
                                                cid = response.Cid;
                                                await _cloudSpeedManager.UpdateFileCid(fileCid.Id, cid, FileCidStatus.Success);
                                                _logger.LogInformation("Ffs stage: {cid}", cid);
                                            }
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        _logger.LogError(0, ex, "powergate ffs stage fail:" + ex.ToString());
                                    }

                                    if (!string.IsNullOrEmpty(cid))
                                    {
                                        await PushStorageConfig(powergateClient, cid, stoppingToken);
                                    }
                                }
                            }
                            skip += limit;
                        }
                    }
                    {
                        int limit = 10;
                        int skip = 0;
                        while (true)
                        {
                            var fileJobs = await _cloudSpeedManager.GetFileJobs(FileJobStatus.Processing, skip, limit);
                            if (fileJobs.Count() == 0)
                            {
                                break;
                            }
                            foreach (var fileJob in fileJobs)
                            {
                                if (fileJob.Updated.AddMinutes(1) > DateTime.Now)
                                    continue;//Too NEW

                                try
                                {
                                    var storageJob = await powergateClient.Ffs.GetStorageJobAsync(new Ffs.Rpc.GetStorageJobRequest { Jid = fileJob.JobId },
                                        powergateClient.BotXFfsToken, cancellationToken: stoppingToken);

                                    if (storageJob == null || storageJob.Job == null)
                                        continue;

                                    if (storageJob.Job.Status == Ffs.Rpc.JobStatus.Executing || storageJob.Job.Status == Ffs.Rpc.JobStatus.Queued)
                                        continue;

                                    if (storageJob.Job.Status == Ffs.Rpc.JobStatus.Canceled)
                                    {
                                        _logger.LogWarning("Ffs job canceled: {jobId} {status}", fileJob.JobId, storageJob.Job.ErrCause);
                                        await _cloudSpeedManager.UpdateFileJob(fileJob.Id, FileJobStatus.Canceled);
                                        continue;
                                    }

                                    if (storageJob.Job.Status == Ffs.Rpc.JobStatus.Failed)
                                    {
                                        _logger.LogWarning("Ffs job failed: {jobId} {status}", fileJob.JobId, storageJob.Job.ErrCause);
                                        await _cloudSpeedManager.UpdateFileJob(fileJob.Id, FileJobStatus.Failed, storageJob.Job.ErrCause);
                                        if (!string.IsNullOrEmpty(storageJob.Job.ErrCause) && storageJob.Job.ErrCause.Contains("all proposals were rejected"))
                                        {
                                            await PushStorageConfig(powergateClient, fileJob.Cid, stoppingToken, true);
                                        }
                                    }
                                    else if (storageJob.Job.Status == Ffs.Rpc.JobStatus.Success)
                                    {
                                        await _cloudSpeedManager.UpdateFileJob(fileJob.Id, FileJobStatus.Success);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Ffs job unknown status: {jobId} {status} {cause}", fileJob.JobId, storageJob.Job.Status, storageJob.Job.ErrCause);
                                    }
                                }
                                catch (Grpc.Core.RpcException ex)
                                {
                                    _logger.LogError(0, "powergate ffs job fail:" + ex.Status.Detail);
                                    if (ex.Status.Detail.Contains("not found"))
                                    {
                                        await _cloudSpeedManager.UpdateFileJob(fileJob.Id, FileJobStatus.Failed, ex.Status.Detail);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    _logger.LogError(0, ex, "powergate ffs job fail:" + ex.ToString());
                                }
                            }
                            skip += limit;
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(0, ex, "PowergateWorker error:" + ex.ToString());
            }
        }

        private async Task PushStorageConfig(PowergateClient powergateClient, string cid, CancellationToken stoppingToken, bool? hasOverride = null)
        {
            try
            {
                var fjId = await _cloudSpeedManager.CreateFileJob(cid);
                var psc = powergateClient.Ffs.PushStorageConfig(new Ffs.Rpc.PushStorageConfigRequest
                {
                    Cid = cid,
                    OverrideConfig = hasOverride ?? false,
                    HasOverrideConfig = hasOverride.HasValue
                }, powergateClient.BotXFfsToken, cancellationToken: stoppingToken);
                if (psc != null)
                {
                    var jobId = psc.JobId;
                    await _cloudSpeedManager.UpdateFileJob(fjId, jobId, FileJobStatus.Processing);
                    _logger.LogInformation("Ffs pushStorageConfig: {cid} {jobId}", cid, jobId);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(0, ex, "powergate ffs pushStorageConfig fail:" + ex.ToString());
            }
        }
    }
}