using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace CloudSpeed.Sdk
{
    public class LotusClient
    {
        private readonly ILogger _logger;
        private readonly LotusClientSetting m_LotusClientSetting;

        public LotusClient(ILogger<LotusClient> logger, LotusClientSetting lotusClientSetting)
        {
            _logger = logger;
            m_LotusClientSetting = lotusClientSetting;
        }

        /*
        {"jsonrpc":"2.0","result":{"Root":{"/":"bafyaa6qsgafcmalqudsaeicm4xetinw2pixca4uelcsjo5wlknw3npuqmlfsgdd76vk6x42mnijaagelucbyabasgafcmalqudsaeiajit4kxcy4j23qxmqso275umu7o4jpwwism262aoyrmeyqrdgizujaaght23voaaikcqeaegeaudu6abjaqcaibaaeecakb2paae"},"ImportID":8},"id":0}
        */
        public async Task<ResponseBase<ClientImportResponse>> ClientImport(ClientImportRequest model)
        {
            var rb = new RequestBase<ClientImportRequest>() { ParamsData = new[] { model }, Method = "Filecoin.ClientImport" };
            return await ExecuteAsync<ClientImportResponse>(rb);
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

        public async Task<ResponseBase<Cid>> ClientStartDeal(ClientStartDealParams model)
        {
            var rb = new RequestBase<ClientStartDealParams>() { Method = "Filecoin.ClientStartDeal", ParamsData = new[] { model } };
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

        public async Task<ResponseBase<SignedStorageAskResponse>> ClientQueryAsk(ClientQueryAskRequest model)
        {
            var rb = new RequestBase<string>() { Method = "Filecoin.ClientQueryAsk", ParamsData = new[] { model.PeerId, model.Miner } };
            return await ExecuteAsync<SignedStorageAskResponse>(rb);
        }

        public async Task<ResponseBase<QueryOffer[]>> ClientFindData(ClientFindDataRequest model)
        {
            var rb = new RequestBase<Cid>() { Method = "Filecoin.ClientFindData", ParamsData = new[] { model.Root, model.Piece } };
            return await ExecuteAsync<QueryOffer[]>(rb);
        }

        private async Task<ResponseBase<T>> ExecuteAsync<T>(RequestBase model)
        {
            var client = new RestClient(m_LotusClientSetting.LotusApi);
            client.Timeout = m_LotusClientSetting.LotusTimeout;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + m_LotusClientSetting.LotusToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                _logger.LogError(0, "{method} code:{code}, message:{message}", model.Method, response.StatusCode, response.Content);
                return ResponseBase<T>.Fail((int)response.StatusCode, response.StatusDescription);
            }
            var data = JsonConvert.DeserializeObject<ResponseBase<T>>(response.Content);
            if (!data.Success)
            {
                _logger.LogError(0, "{method} code:{code}, message:{message}", model.Method, data.Error.Code, data.Error.Message);
            }
            return data;
        }

        private async Task<ResponseBase> ExecuteAsync(RequestBase model)
        {
            return await ExecuteAsync<string>(model);
        }
    }
}
