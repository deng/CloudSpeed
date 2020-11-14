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
        private readonly LotusClientSetting _lotusClientSetting;
        private readonly UploadSetting _uploadSetting;

        public DealerService(ILogger<DealerService> logger, CloudSpeedManager cloudSpeedManager, LotusClientSetting lotusClientSetting, UploadSetting uploadSetting)
        {
            _logger = logger;
            _cloudSpeedManager = cloudSpeedManager;
            _lotusClientSetting = lotusClientSetting;
            _uploadSetting = uploadSetting;
        }

        public async Task<DealResult> OnlineExecute(string fileCidId, ClientStartDealRequest dealRequest)
        {
            _logger.LogInformation("Lotus ClientImport ...");
            var path = _uploadSetting.GetStoragePath(fileCidId);
            var fileSize = new FileInfo(path).Length;
            var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
            var result = await lotusClient.ClientImport(new FileRef
            {
                Path = path,
                IsCAR = false
            });
            var cid = string.Empty;
            long dealSize = 0;
            if (result.Success)
            {
                cid = result.Result.Root.Value;
                await _cloudSpeedManager.UpdateFileCid(fileCidId, cid, FileCidStatus.Success);
                _logger.LogInformation("Lotus ClientImport: {cid}", cid);

                _logger.LogWarning(0, string.Format("client deal size ..."));
                var dealSizeResult = await lotusClient.ClientDealSize(new Cid { Value = cid });
                if (dealSizeResult.Success)
                {
                    dealSize = dealSizeResult.Result.PieceSize;
                    _logger.LogInformation(0, string.Format("client deal size ...{0}", dealSize));
                    await _cloudSpeedManager.UpdateFileCidDealSize(fileCidId, dealSize, dealSizeResult.Result.PayloadSize);
                }
                else
                {
                    _logger.LogWarning(0, "client deal size failed");
                    return new DealResult { Success = false };
                }
            }
            else
            {
                _logger.LogInformation("Lotus client import failed");
                return new DealResult { Success = false };
            }
            
            var askingPrice = (long)((dealRequest.AskingPrice * dealSize) / (1 << 30)) + 1;
            dealRequest.Price = askingPrice.ToString();
            var dealParams = await CreateTTGraphsyncClientStartDealParams(lotusClient, dealRequest);
            if (dealParams == null)
            {
                _logger.LogError(0, string.Format("CreateTTGraphsyncClientStartDealParams empty."));
                return new DealResult { Success = false };
            }
            _logger.LogInformation("lotus client start dealing: {dataCid} (datacid) {miner} {price} {duration}", cid, dealRequest.Miner, dealRequest.Price, dealRequest.Duration);
            var dealCid = await lotusClient.ClientStartDeal(dealParams);
            if (dealCid.Success)
            {
                var did = dealCid.Result.Value;
                _logger.LogInformation("lotus client start deal result: {datacid}(datacid) - {dealcid} (dealcid) {price} {duration}", cid, did, dealRequest.Price, dealRequest.Duration);
                await _cloudSpeedManager.UpdateFileDeal(fileCidId, dealRequest.Miner, did, FileDealStatus.Processing);
                    return new DealResult { Success = true, DealCid = did, DealSize = fileSize, DealDate = DateTime.Now };
            }
            else
            {
                _logger.LogError(0, "lotus client start deal failed: {datacid}(datacid) - (errorMessage)", cid, dealCid.Error.Message);
                return new DealResult { Success = false };
            }
        }

        public async Task<DealResult> OfflineExecute(string fileCidId, ClientStartDealRequest dealRequest, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Lotus ClientImport ...");
            var path = _uploadSetting.GetStoragePath(fileCidId);
            var fileSize = new FileInfo(path).Length;
            var lotusClient = GlobalServices.ServiceProvider.GetService<LotusClient>();
            var result = await lotusClient.ClientImport(new FileRef
            {
                Path = path,
                IsCAR = false
            });
            var cid = string.Empty;
            if (result.Success)
            {
                cid = result.Result.Root.Value;
                await _cloudSpeedManager.UpdateFileCid(fileCidId, cid, FileCidStatus.Success);
                _logger.LogInformation("Lotus ClientImport: {cid}", cid);

                _logger.LogWarning(0, string.Format("client deal size ..."));
                var dealSize = await lotusClient.ClientDealSize(new Cid { Value = cid });
                if (dealSize.Success)
                {
                    _logger.LogWarning(0, string.Format("client deal size ...{0}", dealSize.Result.PieceSize));
                    await _cloudSpeedManager.UpdateFileCidDealSize(fileCidId, dealSize.Result.PieceSize, dealSize.Result.PayloadSize);
                }
            }
            var fdId = string.Empty;
            var pieceCid = string.Empty;
            long pieceSize = 0;
            var carPath = path + ".car";
            if (!string.IsNullOrEmpty(cid))
            {
                _logger.LogInformation("Lotus ClientGenCar: {path}", path);
                var genCar = await lotusClient.ClientGenCar(new FileRef
                {
                    Path = path,
                    IsCAR = false
                }, carPath);
                _logger.LogInformation("Lotus ClientCalcCommP: {carPath}", carPath);
                if (genCar.Success)
                {
                    var commP = await lotusClient.ClientCalcCommP(carPath);
                    if (commP.Success)
                    {
                        pieceCid = commP.Result.Root.Value;
                        pieceSize = commP.Result.Size;
                        fdId = await _cloudSpeedManager.CreateFileDeal(cid, pieceCid, pieceSize);
                    }
                }
            }
            if (!string.IsNullOrEmpty(fdId))
            {
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
                    await _cloudSpeedManager.UpdateFileDeal(fdId, dealRequest.Miner, did, FileDealStatus.Processing);
                    _logger.LogInformation("MarketImportDealData : {miner} - {dealcid} (dealcid) {carPath}", dealRequest.Miner, did, carPath);
                    while (true)
                    {
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
                    await _cloudSpeedManager.UpdateFileDeal(fdId, FileDealStatus.Failed, dealCid.Error.Message);
                    _logger.LogError(0, "lotus client start deal failed: {datacid}(datacid) - {errorMessage}", cid, dealCid.Error.Message);
                    return new DealResult { Success = false };
                }
            }
            return new DealResult { Success = false };
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