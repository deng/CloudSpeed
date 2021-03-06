using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudSpeed.Entities;
using CloudSpeed.Managers;
using CloudSpeed.Sdk;
using CloudSpeed.Services;
using CloudSpeed.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CloudSpeed.BackgroundServices
{
    public class LotusWorker : BackgroundService
    {
        private readonly ILogger<LotusWorker> _logger;
        private readonly CloudSpeedManager _cloudSpeedManager;
        private readonly LotusClientSetting _lotusClientSetting;
        private readonly UploadSetting _uploadSetting;

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
            StorageDealStatus.StorageDealStaged
        };

        private readonly ConcurrentDictionary<string, DateTime> _processingDealIds = new ConcurrentDictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, long> _processingDealIdBytes = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, long> _transferingDealIdBytes = new ConcurrentDictionary<string, long>();
        private readonly DealerService _dealerService;

        public LotusWorker(ILogger<LotusWorker> logger, CloudSpeedManager cloudSpeedManager, LotusClientSetting lotusClientSetting, UploadSetting uploadSetting, DealerService dealerService)
        {
            _logger = logger;
            _cloudSpeedManager = cloudSpeedManager;
            _lotusClientSetting = lotusClientSetting;
            _uploadSetting = uploadSetting;
            _dealerService = dealerService;
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
                                _logger.LogInformation("client start deal  {datacid}(dealid) {dealid}(datacid)", fileDeal.Id, fileDeal.Cid);
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

                                    if (_transferingOrBeforeTransferStatus.Contains(dealInfo.Result.State))
                                    {
                                        if (_processingDealIdBytes.TryGetValue(fileDeal.DealId, out long length))
                                        {
                                            _transferingDealIdBytes.TryAdd(fileDeal.DealId, length);
                                        }
                                    }
                                    else
                                    {
                                        _transferingDealIdBytes.TryRemove(fileDeal.DealId, out _);
                                    }

                                    if (dealInfo.Result.State == StorageDealStatus.StorageDealError ||
                                      dealInfo.Result.State == StorageDealStatus.StorageDealProposalNotFound ||
                                      dealInfo.Result.State == StorageDealStatus.StorageDealProposalRejected)
                                    {
                                        _logger.LogError(0, string.Format("lotus client deal fail: {0} {1} {2} {3}", fileDeal.Miner, fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                        await _cloudSpeedManager.UpdateFileDeal(fileDeal.Id, FileDealStatus.Failed, dealInfo.Result.Message ?? dealInfo.Result.State.ToString());
                                        _processingDealIds.TryRemove(fileDeal.DealId, out _);
                                        _processingDealIdBytes.TryRemove(fileDeal.DealId, out _);
                                        continue;
                                    }
                                    else if (dealInfo.Result.State == StorageDealStatus.StorageDealActive)
                                    {
                                        _logger.LogInformation(string.Format("lotus client deal active: {0} {1} {2}", fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                        await _cloudSpeedManager.UpdateFileDeal(fileDeal.Id, FileDealStatus.Success);
                                        _processingDealIds.TryRemove(fileDeal.DealId, out _);
                                        _processingDealIdBytes.TryRemove(fileDeal.DealId, out _);
                                    }
                                    else
                                    {
                                        _logger.LogDebug(string.Format("lotus client deal status: {0} {1} {2}", fileDeal.Cid, fileDeal.DealId, dealInfo.Result.State.ToString()));
                                    }
                                }
                                catch (Exception ex)
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
            catch (Exception ex)
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
                foreach (var miner in lotusClient.MinerClients)
                {
                    var minerInfo = await lotusClient.StateMinerInfo(new StateMinerInfoRequest { Miner = miner.Key });
                    if (minerInfo.Success)
                    {
                        _logger.LogInformation("ClientQueryAsk for {miner}", miner.Key);
                        var ask = await lotusClient.ClientQueryAsk(new ClientQueryAskRequest
                        {
                            PeerId = minerInfo.Result.PeerId,
                            Miner = miner.Key
                        });
                        if (ask.Success)
                        {
                            _logger.LogInformation("ClientQueryAsk for {miner}: {ask}", miner.Key, JsonConvert.SerializeObject(ask));
                        }
                    }
                }
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
                                var path = _uploadSetting.GetStoragePath(fileCid.Id);
                                if (File.Exists(path))
                                {
                                    var fileSize = new FileInfo(path).Length;
                                    if (fileSize >= _uploadSetting.MaxFileSize && _uploadSetting.MaxFileSize > 0)
                                    {
                                        var error = $"file size should be less that {fileSize} < {_uploadSetting.MaxFileSize} ";
                                        await _cloudSpeedManager.UpdateFileCid(fileCid.Id, FileCidStatus.Failed, error);
                                        _logger.LogWarning("Lotus ClientImport fail : {0} {1}", error, path);
                                        continue;
                                    }
                                    if (fileSize <= _uploadSetting.MinFileSize)
                                    {
                                        var error = $"file size should be more that {fileSize} < {_uploadSetting.MinFileSize} ";
                                        await _cloudSpeedManager.UpdateFileCid(fileCid.Id, FileCidStatus.Failed, error);
                                        _logger.LogWarning("Lotus ClientImport fail : {0} {1}", error, path);
                                        continue;
                                    }
                                    var cid = string.Empty;
                                    try
                                    {
                                        _logger.LogInformation("Lotus ClientImport: {cid}", cid);
                                        var result = await lotusClient.ClientImport(new FileRef
                                        {
                                            Path = path,
                                            IsCAR = false
                                        });
                                        if (result.Success)
                                        {
                                            cid = result.Result.Root.Value;
                                            _logger.LogInformation("Lotus ClientImport: {cid}", cid);
                                            await _cloudSpeedManager.UpdateFileCid(fileCid.Id, cid, FileCidStatus.Success);
                                            _logger.LogWarning(0, string.Format("client deal size ..."));
                                            var dealSize = await lotusClient.ClientDealSize(new Cid { Value = cid });
                                            if (dealSize.Success)
                                            {
                                                _logger.LogWarning(0, string.Format("client deal size ...{0}", dealSize.Result.PieceSize));
                                                await _cloudSpeedManager.UpdateFileCidDealSize(fileCid.Id, dealSize.Result.PieceSize, dealSize.Result.PayloadSize);
                                            }
                                            await _cloudSpeedManager.CreateFileDeal(cid);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(0, ex, "Lotus ClientImport fail:" + ex.ToString());
                                    }
                                }
                            }
                            skip += limit;
                        }
                    }
                    await Task.Delay(5000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "LotusWorker error:" + ex.ToString());
            }
        }

        private async Task ClientStartDeal(LotusClient lotusClient, string fileDealId, string cid, CancellationToken stoppingToken)
        {
            try
            {
                //limit transfering 
                while (_transferingDealIdBytes.Count > 0)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    var keys = _transferingDealIdBytes.Keys.ToArray();
                    var maxSizeDealTransfering = LotusMinerSetting.GetMaxTransferingSizeInBytes(_lotusClientSetting.MaxTransferingSize);
                    if (maxSizeDealTransfering == 0 && _transferingDealIdBytes.Count < _lotusClientSetting.MaxTransferingCount)
                        break;

                    var sizeTransferingDealIds = _transferingDealIdBytes.Sum(a => a.Value);
                    if (sizeTransferingDealIds < maxSizeDealTransfering && _transferingDealIdBytes.Count < _lotusClientSetting.MaxTransferingCount)
                        break;

                    if (sizeTransferingDealIds >= maxSizeDealTransfering)
                    {
                        _logger.LogWarning(0, string.Format("limit transfering by sizes {0} >= {1}(max).", sizeTransferingDealIds, maxSizeDealTransfering));
                    }
                    if (_transferingDealIdBytes.Count > _lotusClientSetting.MaxTransferingCount)
                    {
                        _logger.LogWarning(0, string.Format("limit transfering by count {0} >= {1}(max).", _transferingDealIdBytes.Count, _lotusClientSetting.MaxTransferingCount));
                    }

                    await Task.Delay(10000, stoppingToken);
                }

                if (stoppingToken.IsCancellationRequested)
                    return;

                var fileCid = await _cloudSpeedManager.GetFileCidByCid(cid);
                if (fileCid == null)
                {
                    _logger.LogError(0, string.Format("fileCid not found {0}.", cid));
                    await _cloudSpeedManager.UpdateFileDeal(fileDealId, FileDealStatus.Failed, "fileCid not found");
                    return;
                }
                var fileFullPath = _uploadSetting.GetStoragePath(fileCid.Id);
                if (!File.Exists(fileFullPath))
                {
                    _logger.LogError(0, string.Format("file not found {0}.", cid));
                    await _cloudSpeedManager.UpdateFileDeal(fileDealId, FileDealStatus.Failed, "file not found");
                    return;
                }
                _logger.LogInformation("query file length: {path}", fileFullPath);
                var fileSize = new FileInfo(fileFullPath).Length;

                var online = fileSize < SectorSizeConstants.Bytes512MiB;

                var miner = _lotusClientSetting.GetMinerByFileSize(fileSize, online);

                if (miner == null)
                {
                    online = !online;
                    miner = _lotusClientSetting.GetMinerByFileSize(fileSize, online);
                }

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
                    if (!decimal.TryParse(ask.Result.Price, out askingPrice))
                    {
                        _logger.LogError(0, string.Format("can't parse ask price :{0}.", ask.Result.Price));
                        return;
                    }
                }

                if (askingPrice == 0)
                {
                    _logger.LogError(0, "asking price should be more than zero.");
                    return;
                }

                _logger.LogInformation("will use askingPrice {askingPrice} for miner {miner}", askingPrice, miner.Miner);

                var minDealDuration = 180 * LotusConstants.EpochsInDay;
                var dealRequest = new ClientStartDealRequest
                {
                    DataCid = cid,
                    Miner = miner.Miner,
                    Duration = minDealDuration,
                    AskingPrice = askingPrice
                };

                if (online)
                {
                    await _dealerService.OnlineExecute(fileDealId, fileCid.Id, dealRequest);
                }
                else
                {
                    await _dealerService.OfflineExecute(fileDealId, fileCid.Id, dealRequest, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "lotus client start deal fail:" + ex.ToString());
            }
        }
    }
}