using Newtonsoft.Json;
using RestSharp;

namespace CloudSpeed.Sdk
{
    /*
    {"jsonrpc":"2.0",
    "result":{"Root":{"/":"bafyaa6qsgafcmalqudsaeicm4xetinw2pixca4uelcsjo5wlknw3npuqmlfsgdd76vk6x42mnijaagelucbyabasgafcmalqudsaeiajit4kxcy4j23qxmqso275umu7o4jpwwism262aoyrmeyqrdgizujaaght23voaaikcqeaegeaudu6abjaqcaibaaeecakb2paae"},"ImportID":8},
    "id":0}
    */
    public class ResponseBase<T> : ResponseBase
    {
        [JsonProperty("result")]
        public T Result { get; set; }

        public static new ResponseBase<T> Fail(int code, string messsage)
        {
            return new ResponseBase<T>
            {
                JsonRpc = "2.0",
                Id = 0,
                Error = new ResponseError { Code = code, Message = messsage },
                Result = default(T)
            };
        }
    }

    public class ResponseBase
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("error")]
        public ResponseError Error { get; set; }

        public bool Success
        {
            get { return Error == null; }
        }

        public static ResponseBase Fail(int code, string messsage)
        {
            return new ResponseBase
            {
                JsonRpc = "2.0",
                Id = 0,
                Error = new ResponseError { Code = code, Message = messsage },
            };
        }
    }

    public class ResponseError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
