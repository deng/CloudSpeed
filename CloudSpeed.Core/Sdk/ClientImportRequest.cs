using Newtonsoft.Json;

namespace CloudSpeed.Sdk
{
    /*
    [{\"path\":\"${path}\",\"isCAR\":false}]
    */
    public class ClientImportRequest
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("isCAR")]
        public bool IsCAR { get; set; }
    }
}
