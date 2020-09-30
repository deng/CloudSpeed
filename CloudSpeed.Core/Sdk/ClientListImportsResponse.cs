using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    /*
    {"Root":
        {"/":"bafyaa6qsgafcmalqudsaeicm4xetinw2pixca4uelcsjo5wlknw3npuqmlfsgdd76vk6x42mnijaagelucbyabasgafcmalqudsaeiajit4kxcy4j23qxmqso275umu7o4jpwwism262aoyrmeyqrdgizujaaght23voaaikcqeaegeaudu6abjaqcaibaaeecakb2paae"},
        "ImportID":8
    }
    */

    public class ClientListImportsResponse
    {
        public int Key { get; set; }

        public string Err { get; set; }

        public Cid Root { get; set; }

        public string Source { get; set; }

        public string FilePath { get; set; }
    }
}
