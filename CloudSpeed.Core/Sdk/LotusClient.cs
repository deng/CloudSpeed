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
        public async Task<ResponseBase<ClientImportResponse>> ClientImport(FileRef model)
        {
            var rb = new RequestBase<FileRef>() { ParamsData = new[] { model }, Method = "Filecoin.ClientImport", Timeout = 0 };
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
            var rb = new RequestBase<string>() { ParamsData = new[] { inpath }, Method = "Filecoin.ClientCalcCommP", Timeout = 0 };
            return await ExecuteAsync<CommPRet>(rb);
        }

        public async Task<ResponseBase> ClientGenCar(FileRef model, string outpath)
        {
            var rb = new RequestBase<object>() { ParamsData = new object[] { model, outpath }, Method = "Filecoin.ClientGenCar", Timeout = 0 };
            return await ExecuteAsync(rb);
        }

        private async Task<ResponseBase<T>> ExecuteAsync<T>(RequestBase model)
        {
            var client = new RestClient(m_LotusClientSetting.LotusApi);
            client.Timeout = model.Timeout ?? m_LotusClientSetting.LotusTimeout;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + m_LotusClientSetting.LotusToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                _logger.LogError(0, "{method} fail:", model.Method);
                if (!string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogError(0, "content: {content}", response.Content);
                }
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _logger.LogError(0, "error message: {message}", response.ErrorMessage);
                }
                if (response.StatusCode > 0)
                {
                    _logger.LogError(0, "status code: {code}", response.StatusCode);
                }
                return ResponseBase<T>.Fail((int)response.StatusCode, response.StatusDescription);
            }
            var data = JsonConvert.DeserializeObject<ResponseBase<T>>(response.Content);
            if (!data.Success)
            {
                _logger.LogError(0, "{method} code:{code}, message:{message}", model.Method, data.Error.Code, data.Error.Message);
            }
            if (data.Result == null || model.Debug)
            {
                _logger.LogWarning(0, "{method} Content:{message}", model.Method, response.Content);
            }

            return data;
        }

        private async Task<ResponseBase> ExecuteAsync(RequestBase model)
        {
            return await ExecuteAsync<string>(model);
        }
    }
}
