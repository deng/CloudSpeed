using Newtonsoft.Json;

namespace CloudSpeed.Sdk
{
    public class ClientFindDataRequest
    {
        public Cid Root { get; set; }

        public Cid Piece { get; set; }
    }
}
