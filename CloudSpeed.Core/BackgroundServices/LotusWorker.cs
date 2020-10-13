using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudSpeed.Entities;
using CloudSpeed.Managers;
using CloudSpeed.Powergate;
using CloudSpeed.Sdk;
using CloudSpeed.Services;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudSpeed.BackgroundServices
{
    public class LotusWorker : BackgroundService
    {
        private readonly ILogger<LotusWorker> _logger;
        private readonly CloudSpeedManager _cloudSpeedManager;
        private readonly LotusClientSetting _lotusClientSetting;

        private readonly StorageDealStatus[] _transferingOrBeforeTransferStatus = new StorageDealStatus[]
        {
            StorageDealStatus.StorageDealProposalAccepted,
            StorageDealStatus.StorageDealPublish,
            StorageDealStatus.StorageDealPublishing,
            StorageDealStatus.StorageDealStartDataTransfer,
            StorageDealStatus.StorageDealTransferring,
            StorageDealStatus.StorageDealAcceptWait,
            StorageDealStatus.StorageDealCheckForAcceptance,
            StorageDealStatus.StorageDealClientFunding,
            StorageDealStatus.StorageDealProviderFunding,
            StorageDealStatus.StorageDealEnsureClientFunds,
            StorageDealStatus.StorageDealEnsureProviderFunds,
            StorageDealStatus.StorageDealValidating,
            StorageDealStatus.StorageDealFundsEnsured,
        };

        private readonly ConcurrentDictionary<string, DateTime> _processingDealIds = new ConcurrentDictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, DateTime> _transferingDealIds = new ConcurrentDictionary<string, DateTime>();
        private readonly int _maxCountDealTransfering = 3;

        public LotusWorker(ILogger<LotusWorker> logger, CloudSpeedManager cloudSpeedManager, LotusClientSetting lotusClientSetting)
        {
            _logger = logger;
            _cloudSpeedManager = cloudSpeedManager;
            _lotusClientSetting = lotusClientSetting;
        }

        private async Task CreateSentryBox1(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("create sentry box 1");
                var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    //check for none deals
                    {
                        _logger.LogDebug("check for none deals");
                        int limit = 10;
                        int skip = 0;
                        while (true)
                        {
                            var fileDeals = await _cloudSpeedManager.GetFileDeals(FileDealStatus.None, skip, limit);
                            if (fileDeals.Count() == 0)
                            {
                                _logger.LogDebug("file deal empty with status none");
                                break;
                            }
                            _logger.LogInformation("found file {count} deals", fileDeals.Count());
                            foreach (var fileDeal in fileDeals)
                            {
                                await ClientStartDeal(lotusClient, fileDeal.Id, fileDeal.Cid, stoppingToken);
                            }
                            skip += limit;
                        }
                    }
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(0, ex, "SentryBox error:" + ex.ToString());
            }
        }

        private async Task CreateSentryBox2(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("create sentry box 2");
                var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    //check for processing deals
                    {
                        _logger.LogDebug("check for processing deals");
                        int limit = 10;
                        int skip = 0;
                        while (true)
                        {
                            var fileDeals = await _cloudSpeedManager.GetFileDeals(FileDealStatus.Processing, skip, limit);
                            if (fileDeals.Count() == 0)
                            {
                                _logger.LogDebug("file deal empty with status procesing");
                                break;
                            }
                            foreach (var fileDeal in fileDeals)
                            {
                                if (fileDeal.Updated.AddMinutes(1) > DateTime.Now)
                                    continue; //Too NEW

                                try
                                {
                                    var dealInfo = await lotusClient.ClientGetDealInfo(new Cid { Value = fileDeal.DealId });
                                    if (!dealInfo.Success || dealInfo.Result == null)
                                    {
                                        _logger.LogError(0, string.Format("lotus client deal fail: {0} {1} {2}", fileDeal.Cid, fileDeal.DealId, dealInfo.Error));
                                        continue;
                                    }
                                    else if (dealInfo.Result.State == StorageDealStatus.StorageDealError ||
                                      dealInfo.Result.State == StorageDealStatus.StorageDealProposalNotFound ||
                                      dealInfo.Result.State == StorageDealStatus.StorageDealProposalRejected)
                                    {
                                        _logger.LogError(0, string.Format("lotus client deal fail: {0} {1} {2} {3}", fileDeal.Miner, fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                        await _cloudSpeedManager.UpdateFileDeal(fileDeal.Id, FileDealStatus.Failed, dealInfo.Result.Message ?? dealInfo.Result.State.ToString());
                                        _processingDealIds.TryRemove(fileDeal.DealId, out _);
                                        continue;
                                    }
                                    else if (dealInfo.Result.State == StorageDealStatus.StorageDealActive)
                                    {
                                        _logger.LogInformation(string.Format("lotus client deal active: {0} {1} {2}", fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                        await _cloudSpeedManager.UpdateFileDeal(fileDeal.Id, FileDealStatus.Success);
                                        _processingDealIds.TryRemove(fileDeal.DealId, out _);
                                    }
                                    else
                                    {
                                        _logger.LogDebug(string.Format("lotus client deal status: {0} {1} {2}", fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
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
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(0, ex, "SentryBox error:" + ex.ToString());
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LotusWorker running at: {time}", DateTimeOffset.Now);
            var _1 = CreateSentryBox1(stoppingToken);
            var _2 = CreateSentryBox2(stoppingToken);
            try
            {
                var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
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
                                _logger.LogDebug(0, "file cid empty with status none");
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
                                    }
                                }
                            }
                            skip += limit;
                        }
                    }
                    await Task.Delay(5000, stoppingToken);
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
                //limit transfering 
                while (_processingDealIds.Count(a => a.Value >= DateTime.Now.AddHours(-1)) > 0)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    foreach (var processingDealId in _processingDealIds.Keys)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;
                        var dealInfo = await lotusClient.ClientGetDealInfo(new Cid { Value = processingDealId });
                        if (dealInfo.Result != null)
                        {
                            if (_transferingOrBeforeTransferStatus.Contains(dealInfo.Result.State))
                            {
                                if (!_transferingDealIds.ContainsKey(processingDealId))
                                {
                                    _transferingDealIds.TryAdd(processingDealId, DateTime.Now);
                                }
                            }
                            else
                            {
                                _transferingDealIds.TryRemove(processingDealId, out _);
                            }
                        }
                        await Task.Delay(1000, stoppingToken);
                    }

                    var countTransferingDealIds = _transferingDealIds.Count(a => a.Value >= DateTime.Now.AddHours(-1));
                    if (countTransferingDealIds < _maxCountDealTransfering)
                        break;

                    _logger.LogWarning(0, string.Format("limit transfering {0}.", countTransferingDealIds));
                    await Task.Delay(10000, stoppingToken);
                }

                if (stoppingToken.IsCancellationRequested)
                    return;

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
                _logger.LogInformation("query file length: {path}", fileFullPath);
                var fileSize = new FileInfo(fileFullPath).Length;
                var miner = _lotusClientSetting.GetMinerByFileSize(fileSize);
                if (miner == null)
                {
                    _logger.LogError(0, string.Format("can't found any miner for :{0} {1}.", cid, fileSize));
                    return;
                }

                var minerInfo = await lotusClient.StateMinerInfo(new StateMinerInfoRequest { Miner = miner.Miner });
                if (!minerInfo.Success)
                {
                    _logger.LogError(0, string.Format("can't get state info from :{0}.", miner));
                    return;
                }
                var askingPrice = miner.AskingPrice;
                if (askingPrice == 0)
                {
                    var ask = await lotusClient.ClientQueryAsk(new ClientQueryAskRequest
                    {
                        PeerId = minerInfo.Result.PeerId,
                        Miner = miner.Miner
                    });
                    if (!ask.Success)
                    {
                        _logger.LogError(0, string.Format("can't query ask from :{0} {1}.", minerInfo.Result.PeerId, miner));
                        return;
                    }
                    if (!decimal.TryParse(ask.Result.Price, out decimal arp))
                    {
                        _logger.LogError(0, string.Format("can't parse ask price :{0}.", ask.Result.Price));
                        return;
                    }
                    askingPrice = arp;
                    if (askingPrice == 0)
                    {
                        _logger.LogError(0, "asking price should be more than zero.");
                        return;
                    }
                }
                var minDealDuration = 180 * LotusConstants.EpochsInDay;
                var dealRequest = new ClientStartDealRequest
                {
                    DataCid = cid,
                    Miner = miner.Miner,
                    Price = askingPrice.ToString(), //Price per GiB
                    Duration = minDealDuration
                };
                var dealParams = await CreateTTGraphsyncClientStartDealParams(lotusClient, dealRequest);
                if (dealParams == null)
                {
                    _logger.LogError(0, string.Format("CreateTTGraphsyncClientStartDealParams empty."));
                    return;
                }
                _logger.LogInformation("lotus client start dealing: {dataCid} (datacid) {miner} {price} {duration}", cid, dealRequest.Miner, dealRequest.Price, dealRequest.Duration);
                var dealCid = await lotusClient.ClientStartDeal(dealParams);
                if (dealCid.Success)
                {
                    var did = dealCid.Result.Value;
                    _logger.LogInformation("lotus client start deal result: {datacid}(datacid) - {dealcid} (dealcid) {price} {duration}", cid, did, dealRequest.Price, dealRequest.Duration);
                    await _cloudSpeedManager.UpdateFileDeal(fdId, miner.Miner, did, FileDealStatus.Processing);
                    _processingDealIds.AddOrUpdate(did, key => DateTime.Now, (key, oldDt) => DateTime.Now);
                }
                else
                {
                    _logger.LogError(0, "lotus client start deal failed: {datacid}(datacid) - (errorMessage)", cid, dealCid.Error.Message);
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
                _logger.LogError(0, "can't get wallet default address.");
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