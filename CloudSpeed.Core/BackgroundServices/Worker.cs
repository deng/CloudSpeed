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
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly LotusClient _lotusClient;
        private readonly LotusClientSetting _lotusClientSetting;
        private readonly CloudSpeedManager _cloudSpeedManager;

        public Worker(ILogger<Worker> logger, LotusClient lotusClient, LotusClientSetting lotusClientSetting, CloudSpeedManager cloudSpeedManager)
        {
            _logger = logger;
            _lotusClient = lotusClient;
            _lotusClientSetting = lotusClientSetting;
            _cloudSpeedManager = cloudSpeedManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var powergateClient = GlobalServices.ServiceProvider.GetService<PowergateClient>();
                var ffsInfo = await powergateClient.Ffs.InfoAsync(new Ffs.Rpc.InfoRequest());
                _logger.LogInformation("Ffs info: {info}", Newtonsoft.Json.JsonConvert.SerializeObject(ffsInfo.Info));
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
                                using (var stage = powergateClient.Ffs.Stage())
                                {
                                    using (var fsRead = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, UploadConstants.BigFileWriteSize, true))
                                    {
                                        var buffer = new byte[UploadConstants.BigFileWriteSize];
                                        var readCount = 0;
                                        while ((readCount = await fsRead.ReadAsync(buffer, 0, buffer.Length)) > 0)
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
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                _logger.LogError(0, ex, "powergate ffs stage fail:" + ex.ToString());
                            }

                            if (!string.IsNullOrEmpty(cid))
                            {
                                try
                                {
                                    await _cloudSpeedManager.CreateFileJob(fileCid.Id, cid);
                                    var psc = powergateClient.Ffs.PushStorageConfig(new Ffs.Rpc.PushStorageConfigRequest { Cid = cid });
                                    if (psc != null)
                                    {
                                        var jobId = psc.JobId;
                                        await _cloudSpeedManager.UpdateFileJob(fileCid.Id, jobId, FileJobStatus.Processing);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    _logger.LogError(0, ex, "powergate ffs pushStorageConfig fail:" + ex.ToString());
                                }
                            }
                        }
                    }
                    skip += limit;
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<ClientStartDealParams> CreateTTGraphsyncClientStartDealParams(ClientStartDealRequest model)
        {
            var dataRef = new TransferDataRef()
            {
                TransferType = TransferType.graphsync.ToString(),
                Root = new Cid() { Value = model.DataCid },
            };

            var walletAddress = await _lotusClient.WalletDefaultAddress();
            if (!walletAddress.Success)
            {
                return null;
            }
            var dealParams = new ClientStartDealParams()
            {
                Data = dataRef,
                Miner = model.Miner,
                MinBlocksDuration = (ulong)model.Duration,
                EpochPrice = model.Price,
                Wallet = walletAddress.Result,
                VerifiedDeal = false,
                ProviderCollateral = "0"
            };
            return dealParams;
        }
    }
}