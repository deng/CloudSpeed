using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    /*
    {"/":"bafk2bzacebhgxx676o5asbbrtegsl4orgpzke22ezeqvuyvmptf4y5g2o45za"}
    */
    public class Cid
    {
        [JsonProperty("/")]
        public string Value { get; set; }
    }

    public class TipSetKey
    {
        public static TipSetKey EmptyTSK = new TipSetKey() { Value = "" };

        public string Value { get; set; }
    }
}
