using Newtonsoft.Json;

namespace CloudSpeed.Sdk
{
    /*
    [{\"path\":\"${path}\",\"isCAR\":false}]
    */
    public class FileRef
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("isCAR")]
        public bool IsCAR { get; set; }
    }
}
