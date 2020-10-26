using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public class LotusApi
    {
        public string Api { get; set; }

        public string Token { get; set; }

        public int Timeout { get; set; } = -1;
    }
}
