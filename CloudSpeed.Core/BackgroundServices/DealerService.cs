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
    public class DealResult
    {
        public string DealCid { get; set; }

        public DateTime DealDate { get; set; }

        public long DealSize { get; set; }

        public bool Success { get; set; }
    }

    public class DealerService
    {
        private readonly ILogger<DealerService> _logger;
        private readonly CloudSpeedManager _cloudSpeedManager;
        private readonly UploadSetting _uploadSetting;

        public DealerService(ILogger<DealerService> logger, CloudSpeedManager cloudSpeedManager, UploadSetting uploadSetting)
        {
            _logger = logger;
            _cloudSpeedManager = cloudSpeedManager;
            _uploadSetting = uploadSetting;
        }

        public async Task<DealResult> OnlineExecute(string fileDealId, string fileCidId, ClientStartDealRequest dealRequest)
        {
            var path = _uploadSetting.GetStoragePath(fileCidId);
            if (!File.Exists(path))
            {
                _logger.LogError("data not found " + path);
                return new DealResult { Success = false };
            }
            var fileCid = await _cloudSpeedManager.GetFileCid(fileCidId);
            string cid = fileCid.Cid;
            if (string.IsNullOrEmpty(cid))
            {
                _logger.LogError("data cid is required");
                return new DealResult { Success = false };
            }
            var fileSize = new FileInfo(path).Length;
            var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
            long dealSize = fileCid.DealSize;
            if (dealSize == 0)
            {
                _logger.LogInformation("client deal size ...");
                var dealSizeResult = await lotusClient.ClientDealSize(new Cid { Value = cid });
                if (dealSizeResult.Success)
                {
                    dealSize = dealSizeResult.Result.PieceSize;
                    _logger.LogInformation(string.Format("client deal size ...{0}", dealSize));
                    await _cloudSpeedManager.UpdateFileCidDealSize(fileCidId, dealSize, dealSizeResult.Result.PayloadSize);
                }
                else
                {
                    _logger.LogError("client deal size failed: " + dealSizeResult.Error?.Message);
                    return new DealResult { Success = false };
                }
            }
            dealRequest.CalPriceByDealSize(dealSize);
            var dealParams = await CreateTTGraphsyncClientStartDealParams(lotusClient, dealRequest);
            if (dealParams == null)
            {
                _logger.LogError(string.Format("CreateTTGraphsyncClientStartDealParams empty."));
                return new DealResult { Success = false };
            }
            _logger.LogInformation("lotus client start dealing: {dataCid} (datacid) {miner} {price} {duration}", cid, dealRequest.Miner, dealRequest.Price, dealRequest.Duration);
            var dealCid = await lotusClient.ClientStartDeal(dealParams);
            if (dealCid.Success)
            {
                var did = dealCid.Result.Value;
                _logger.LogInformation("lotus client start deal result: {datacid}(datacid) - {dealcid} (dealcid) {price} {duration}", cid, did, dealRequest.Price, dealRequest.Duration);
                await _cloudSpeedManager.UpdateFileDeal(fileDealId, dealRequest.Miner, did, FileDealStatus.Processing);
                return new DealResult { Success = true, DealCid = did, DealSize = fileSize, DealDate = DateTime.Now };
            }
            else
            {
                await _cloudSpeedManager.UpdateFileDeal(fileDealId, FileDealStatus.Failed, dealCid.Error.Message);
                _logger.LogError("lotus client start deal failed: {datacid}(datacid) - (errorMessage)", cid, dealCid.Error?.Message);
                return new DealResult { Success = false };
            }
        }

        public async Task<DealResult> OfflineExecute(string fileDealId, string fileCidId, ClientStartDealRequest dealRequest, CancellationToken stoppingToken)
        {
            var path = _uploadSetting.GetStoragePath(fileCidId);
            if (!File.Exists(path))
            {
                _logger.LogError("data not found " + path);
                return new DealResult { Success = false };
            }
            var fileCid = await _cloudSpeedManager.GetFileCid(fileCidId);
            string cid = fileCid.Cid;
            if (string.IsNullOrEmpty(cid))
            {
                _logger.LogError("data cid is required");
                return new DealResult { Success = false };
            }
            var fileSize = new FileInfo(path).Length;
            var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
            long dealSize = fileCid.DealSize;
            if (dealSize == 0)
            {
                _logger.LogInformation("client deal size ...");
                var dealSizeResult = await lotusClient.ClientDealSize(new Cid { Value = cid });
                if (dealSizeResult.Success)
                {
                    dealSize = dealSizeResult.Result.PieceSize;
                    _logger.LogInformation(string.Format("client deal size ...{0}", dealSize));
                    await _cloudSpeedManager.UpdateFileCidDealSize(fileCidId, dealSize, dealSizeResult.Result.PayloadSize);
                }
                else
                {
                    _logger.LogError("client deal size failed: " + dealSizeResult.Error?.Message);
                    return new DealResult { Success = false };
                }
            }
            dealRequest.CalPriceByDealSize(dealSize);
            var pieceCid = fileCid.PieceCid;
            long pieceSize = fileCid.PieceSize;
            var carPath = path + ".car";
            if (!File.Exists(carPath))
            {
                _logger.LogInformation("Lotus ClientGenCar: {path}", path);
                var genCar = await lotusClient.ClientGenCar(new FileRef
                {
                    Path = path,
                    IsCAR = false
                }, carPath);
                if (!genCar.Success)
                {
                    _logger.LogError("client ClientGenCar failed: " + genCar.Error?.Message);
                    return new DealResult { Success = false };
                }
            }
            if (string.IsNullOrEmpty(pieceCid))
            {
                _logger.LogInformation("Lotus ClientCalcCommP: {carPath}", carPath);
                var commP = await lotusClient.ClientCalcCommP(carPath);
                if (commP.Success)
                {
                    pieceCid = commP.Result.Root.Value;
                    pieceSize = commP.Result.Size;
                    await _cloudSpeedManager.UpdateFileCidCommP(fileCid.Id, pieceCid, pieceSize);
                }
                else
                {
                    _logger.LogError("client ClientCalcCommP failed: " + commP.Error?.Message);
                    return new DealResult { Success = false };
                }
            }

            var dealParams = await CreateTTManualClientStartDealParams(lotusClient, dealRequest, new CommPRet
            {
                Root = new Cid { Value = pieceCid },
                Size = pieceSize
            });
            if (dealParams == null)
            {
                _logger.LogError(0, string.Format("CreateTTManualClientStartDealParams empty."));
                return new DealResult { Success = false };
            }
            _logger.LogInformation("lotus client start dealing: {dataCid} (datacid) {miner} {price} {duration}", cid, dealRequest.Miner, dealRequest.Price, dealRequest.Duration);
            var dealCid = await lotusClient.ClientStartDeal(dealParams);
            if (dealCid.Success)
            {
                var did = dealCid.Result.Value;
                _logger.LogInformation("lotus client start deal result: {datacid}(datacid) - {dealcid} (dealcid) {price} {duration}", cid, did, dealRequest.Price, dealRequest.Duration);
                await _cloudSpeedManager.UpdateFileDeal(fileDealId, dealRequest.Miner, did, FileDealStatus.Processing);
                _logger.LogInformation("MarketImportDealData : {miner} - {dealcid} (dealcid) {carPath}", dealRequest.Miner, did, carPath);
                var startImport = DateTime.Now;
                while (true)
                {
                    var timeout = DateTime.Now - startImport;
                    if (timeout.TotalMinutes > 30)
                    {
                        _logger.LogWarning("market import deal data fail: {miner} - {dealcid} (dealcid) {carPath} timeout", dealRequest.Miner, did, carPath);
                        return new DealResult { Success = false };
                    }
                    await Task.Delay(15000, stoppingToken);
                    var importResult = await lotusClient.MinerClients[dealRequest.Miner].MarketImportDealData(new Cid { Value = did }, carPath);
                    if (importResult.Success)
                    {
                        _logger.LogInformation("market import deal data success: {miner} - {dealcid} (dealcid) {carPath}", dealRequest.Miner, did, carPath);
                        break;
                    }
                    _logger.LogWarning("market import deal data fail: {miner} - {dealcid} (dealcid) {carPath}", dealRequest.Miner, did, carPath);
                }
                return new DealResult { Success = true, DealCid = did, DealSize = fileSize, DealDate = DateTime.Now };
            }
            else
            {
                await _cloudSpeedManager.UpdateFileDeal(fileDealId, FileDealStatus.Failed, dealCid.Error.Message);
                _logger.LogError(0, "lotus client start deal failed: {datacid}(datacid) - {errorMessage}", cid, dealCid.Error.Message);
                return new DealResult { Success = false };
            }
        }

        private async Task<ClientStartDealParams> CreateTTManualClientStartDealParams(LotusClient lotusClient, ClientStartDealRequest model, CommPRet commPRet)
        {
            var dataRef = new TransferDataRef()
            {
                TransferType = TransferType.manual.ToString(),
                Root = new Cid() { Value = model.DataCid },
                PieceCid = commPRet.Root,
                PieceSize = commPRet.Size
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