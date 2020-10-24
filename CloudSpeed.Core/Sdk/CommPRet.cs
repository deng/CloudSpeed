using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public class CommPRet
    {
        [JsonProperty("Root")]
        public Cid Root { get; set; }

        [JsonProperty("Size")]
        public int Size { get; set; }
    }
}
