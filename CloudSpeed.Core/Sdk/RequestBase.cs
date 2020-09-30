using Newtonsoft.Json;
using RestSharp;

namespace CloudSpeed.Sdk
{
    /*
    "{\"jsonrpc\":\"2.0\",\"method\":\"Filecoin.ClientImport\",\"params\":[{\"path\":\"${path}\",\"isCAR\":false}],\"id\":0}"
    */
    public class RequestBase<T> : RequestBase
    {

        [JsonProperty("params")]
        public T[] ParamsData { get; set; }
    }

    public class RequestBase
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
