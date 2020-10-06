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
    public class LotusWorker : BackgroundService
    {
        private readonly ILogger<LotusWorker> _logger;
        private readonly CloudSpeedManager _cloudSpeedManager;
        private readonly LotusClientSetting _lotusClientSetting;

        public LotusWorker(ILogger<LotusWorker> logger, CloudSpeedManager cloudSpeedManager, LotusClientSetting lotusClientSetting)
        {
            _logger = logger;
            _cloudSpeedManager = cloudSpeedManager;
            _lotusClientSetting = lotusClientSetting;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LotusWorker running at: {time}", DateTimeOffset.Now);
            var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
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
                                        var result = await lotusClient.ClientImport(new ClientImportRequest
                                        {
                                            Path = path,
                                            IsCAR = false
                                        });
                                        if (result.Success)
                                        {
                                            cid = result.Result.Root.Value;
                                            await _cloudSpeedManager.UpdateFileCid(fileCid.Id, cid, FileCidStatus.Success);
                                            _logger.LogInformation("Lotus ClientImport: {cid}", cid);
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        _logger.LogError(0, ex, "Lotus ClientImport fail:" + ex.ToString());
                                    }
                                    if (!string.IsNullOrEmpty(cid))
                                    {
                                        var fdId = await _cloudSpeedManager.CreateFileDeal(cid);
                                        await ClientStartDeal(lotusClient, fdId, cid, stoppingToken);
                                    }
                                }
                            }
                            skip += limit;
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                    //check for none deals
                    {
                        int limit = 10;
                        int skip = 0;
                        while (true)
                        {
                            var fileDeals = await _cloudSpeedManager.GetFileDeals(FileDealStatus.None, skip, limit);
                            if (fileDeals.Count() == 0)
                            {
                                break;
                            }
                            foreach (var fileDeal in fileDeals)
                            {
                                await ClientStartDeal(lotusClient, fileDeal.Id, fileDeal.Cid, stoppingToken);
                            }
                            skip += limit;
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                    //check for processing deals
                    {
                        int limit = 10;
                        int skip = 0;
                        while (true)
                        {
                            var fileDeals = await _cloudSpeedManager.GetFileDeals(FileDealStatus.Processing, skip, limit);
                            if (fileDeals.Count() == 0)
                            {
                                break;
                            }
                            foreach (var fileDeal in fileDeals)
                            {
                                if (fileDeal.Updated.AddMinutes(1) > DateTime.Now)
                                    continue;//Too NEW

                                try
                                {
                                    var dealInfo = await lotusClient.ClientGetDealInfo(new Cid { Value = fileDeal.DealId });
                                    if (!dealInfo.Success || dealInfo.Result == null)
                                    {
                                        _logger.LogError(0, string.Format("lotus client deal fail: {0} {1}", fileDeal.Cid, fileDeal.DealId, dealInfo.Error));
                                        continue;
                                    }

                                    if (dealInfo.Result.State == StorageDealStatus.StorageDealError ||
                                        dealInfo.Result.State == StorageDealStatus.StorageDealProposalNotFound ||
                                        dealInfo.Result.State == StorageDealStatus.StorageDealProposalRejected)
                                    {
                                        _logger.LogError(0, string.Format("lotus client deal fail: {0} {1}", fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                        await _cloudSpeedManager.UpdateFileDeal(fileDeal.Id, FileDealStatus.Failed, dealInfo.Result.Message ?? dealInfo.Result.State.ToString());
                                        continue;
                                    }

                                    if (dealInfo.Result.State == StorageDealStatus.StorageDealActive)
                                    {
                                        _logger.LogInformation(0, string.Format("lotus client deal active: {0} {1}", fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                        await _cloudSpeedManager.UpdateFileDeal(fileDeal.Id, FileDealStatus.Success);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    _logger.LogError(0, ex, "lotus client get deal info fail:" + ex.ToString());
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
                _logger.LogError(0, ex, "LotusWorker error:" + ex.ToString());
            }
        }

        private async Task ClientStartDeal(LotusClient lotusClient, string fdId, string cid, CancellationToken stoppingToken)
        {
            try
            {
                var fileCid = await _cloudSpeedManager.GetFileCidByCid(cid);
                if (fileCid == null)
                {
                    _logger.LogError(0, string.Format("fileCid not found {0}.", cid));
                    await _cloudSpeedManager.UpdateFileDeal(fdId, FileDealStatus.Failed, "fileCid not found");
                    return;
                }
                var fileFullPath = _cloudSpeedManager.GetStoragePath(fileCid.Id);
                if (!File.Exists(fileFullPath))
                {
                    _logger.LogError(0, string.Format("file not found {0}.", cid));
                    await _cloudSpeedManager.UpdateFileDeal(fdId, FileDealStatus.Failed, "file not found");
                    return;
                }

                var fileSize = new FileInfo(fileFullPath).Length;
                var miner = _lotusClientSetting.GetMinerByFileSize(fileSize);
                if (string.IsNullOrEmpty(miner))
                {
                    _logger.LogError(0, string.Format("can't found any miner for :{0} {1}.", cid, fileSize));
                    return;
                }

                var minerInfo = await lotusClient.StateMinerInfo(new StateMinerInfoRequest { Miner = miner });
                if (!minerInfo.Success)
                {
                    return;
                }
                var ask = await lotusClient.ClientQueryAsk(new ClientQueryAskRequest
                {
                    PeerId = minerInfo.Result.PeerId,
                    Miner = miner
                });
                if (!ask.Success)
                {
                    return;
                }
                var minDealDuration = 180 * LotusConstants.EpochsInDay;
                var dealParams = await CreateTTGraphsyncClientStartDealParams(lotusClient, new ClientStartDealRequest
                {
                    DataCid = cid,
                    Miner = miner,
                    Price = ask.Result.Price,
                    Duration = minDealDuration
                });
                if (dealParams == null)
                {
                    return;
                }
                var dealCid = await lotusClient.ClientStartDeal(dealParams);
                if (dealCid.Success)
                {
                    var did = dealCid.Result.Value;
                    _logger.LogInformation("lot client start deal result: {datacid}(datacid) - {dealcid} (dealcid)", cid, did);
                    await _cloudSpeedManager.UpdateFileDeal(fdId, miner, did, FileDealStatus.Processing);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(0, ex, "lotus client start deal fail:" + ex.ToString());
            }
        }

        private async Task<ClientStartDealParams> CreateTTGraphsyncClientStartDealParams(LotusClient lotusClient, ClientStartDealRequest model)
        {
            var dataRef = new TransferDataRef()
            {
                TransferType = TransferType.graphsync.ToString(),
                Root = new Cid() { Value = model.DataCid },
            };

            var walletAddress = await lotusClient.WalletDefaultAddress();
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