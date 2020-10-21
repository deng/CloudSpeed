using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public class DataSize
    {
        public long PayloadSize { get; set; }

        public long PieceSize { get; set; }
    }
}
