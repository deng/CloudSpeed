using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CloudSpeed.Sdk
{
    public class LotusClient : BaseClient
    {
        private readonly LotusClientSetting m_LotusClientSetting;
        private readonly IDictionary<string, MinerClient> m_MinerClients;

        public LotusClient(ILoggerFactory loggerFactory, LotusClientSetting lotusClientSetting)
            : base(loggerFactory.CreateLogger<LotusClient>(), lotusClientSetting.Api)
        {
            m_LotusClientSetting = lotusClientSetting;
            m_MinerClients = new Dictionary<string, MinerClient>();
            if (lotusClientSetting.Miners != null)
            {
                foreach (var miner in lotusClientSetting.Miners)
                {
                    if (miner.Api != null)
                    {
                        m_MinerClients.Add(miner.Miner, new MinerClient(loggerFactory.CreateLogger<MinerClient>(), miner.Api));
                    }
                }
            }
        }

        public IDictionary<string, MinerClient> MinerClients { get { return m_MinerClients; } }

        /*
        {"jsonrpc":"2.0","result":{"Root":{"/":"bafyaa6qsgafcmalqudsaeicm4xetinw2pixca4uelcsjo5wlknw3npuqmlfsgdd76vk6x42mnijaagelucbyabasgafcmalqudsaeiajit4kxcy4j23qxmqso275umu7o4jpwwism262aoyrmeyqrdgizujaaght23voaaikcqeaegeaudu6abjaqcaibaaeecakb2paae"},"ImportID":8},"id":0}
        */
        public async Task<ResponseBase<ClientImportResponse>> ClientImport(FileRef model)
        {
            var rb = new RequestBase<FileRef>() { ParamsData = new[] { model }, Method = "Filecoin.ClientImport", Timeout = -1 };
            return await ExecuteAsync<ClientImportResponse>(rb);
        }

        public async Task<ResponseBase<DataSize>> ClientDealSize(Cid model)
        {
            var rb = new RequestBase<Cid>() { ParamsData = new[] { model }, Method = "Filecoin.ClientDealSize", Debug = true };
            return await ExecuteAsync<DataSize>(rb);
        }

        /*
        {"jsonrpc":"2.0","method":"Filecoin.ClientHasLocal","params":[{"/":"bafk2bzacebhgxx676o5asbbrtegsl4orgpzke22ezeqvuyvmptf4y5g2o45za"}],"id":0}
        */
        public async Task<ResponseBase<bool>> ClientHasLocal(Cid model)
        {
            var rb = new RequestBase<Cid>() { ParamsData = new[] { model }, Method = "Filecoin.ClientHasLocal" };
            return await ExecuteAsync<bool>(rb);
        }

        /*
        [{
            "Key": 6,
            "Err": "",
            "Root": {
                "/": "bafyaa6qsgafcmalqudsaeicm4xetinw2pixca4uelcsjo5wlknw3npuqmlfsgdd76vk6x42mnijaagelucbyabasgafcmalqudsaeiajit4kxcy4j23qxmqso275umu7o4jpwwism262aoyrmeyqrdgizujaaght23voaaikcqeaegeaudu6abjaqcaibaaeecakb2paae"
            },
            "Source": "import",
            "FilePath": "/home/derek/Windows_xp_.2020.iso"
        }]
        */
        public async Task<ResponseBase<ClientListImportsResponse[]>> ClientListImports()
        {
            var rb = new RequestBase() { Method = "Filecoin.ClientListImports" };
            return await ExecuteAsync<ClientListImportsResponse[]>(rb);
        }

        public async Task<ResponseBase> ClientRemoveImport(int storeID)
        {
            var rb = new RequestBase<int>() { Method = "Filecoin.ClientRemoveImport", ParamsData = new[] { storeID } };
            return await ExecuteAsync(rb);
        }

        public async Task<ResponseBase<ClientGetDealInfoReponse>> ClientGetDealInfo(Cid model)
        {
            var rb = new RequestBase<Cid>() { ParamsData = new[] { model }, Method = "Filecoin.ClientGetDealInfo" };
            return await ExecuteAsync<ClientGetDealInfoReponse>(rb);
        }

        public async Task<ResponseBase<Cid>> ClientStartDeal(ClientStartDealParams model)
        {
            var rb = new RequestBase<ClientStartDealParams>() { Method = "Filecoin.ClientStartDeal", ParamsData = new[] { model }, Timeout = -1 };
            return await ExecuteAsync<Cid>(rb);
        }

        public async Task<ResponseBase<string>> WalletDefaultAddress()
        {
            var rb = new RequestBase() { Method = "Filecoin.WalletDefaultAddress" };
            return await ExecuteAsync<string>(rb);
        }

        public async Task<ResponseBase<MinerInfo>> StateMinerInfo(StateMinerInfoRequest model)
        {
            var rb = new RequestBase<object>() { Method = "Filecoin.StateMinerInfo", ParamsData = new object[] { model.Miner, new object[0] } };
            return await ExecuteAsync<MinerInfo>(rb);
        }

        public async Task<ResponseBase<StorageAsk>> ClientQueryAsk(ClientQueryAskRequest model)
        {
            var rb = new RequestBase<string>() { Method = "Filecoin.ClientQueryAsk", ParamsData = new[] { model.PeerId, model.Miner } };
            return await ExecuteAsync<StorageAsk>(rb);
        }

        public async Task<ResponseBase<QueryOffer[]>> ClientFindData(ClientFindDataRequest model)
        {
            var rb = new RequestBase<Cid>() { Method = "Filecoin.ClientFindData", ParamsData = new[] { model.Root, model.Piece } };
            return await ExecuteAsync<QueryOffer[]>(rb);
        }

        public async Task<ResponseBase<CommPRet>> ClientCalcCommP(string inpath)
        {
            var rb = new RequestBase<string>() { ParamsData = new[] { inpath }, Method = "Filecoin.ClientCalcCommP", Timeout = -1 };
            return await ExecuteAsync<CommPRet>(rb);
        }

        public async Task<ResponseBase> ClientGenCar(FileRef model, string outpath)
        {
            var rb = new RequestBase<object>() { ParamsData = new object[] { model, outpath }, Method = "Filecoin.ClientGenCar", Timeout = -1 };
            return await ExecuteAsync(rb);
        }
    }
}
