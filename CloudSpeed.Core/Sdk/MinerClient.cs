using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace CloudSpeed.Sdk
{
    public class MinerClient : BaseClient
    {
        public MinerClient(ILogger<MinerClient> logger, LotusApi api)
            : base(logger, api)
        {

        }

        public async Task<ResponseBase> MarketImportDealData(Cid cid, string path)
        {
            var rb = new RequestBase<object>() { ParamsData = new object[] { cid, path }, Method = "Filecoin.MarketImportDealData", Timeout = 0 };
            return await ExecuteAsync(rb);
        }
    }
}
