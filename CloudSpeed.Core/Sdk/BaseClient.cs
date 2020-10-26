using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace CloudSpeed.Sdk
{
    public class BaseClient
    {
        private readonly ILogger _logger;
        private readonly LotusApi _api;

        protected ILogger Logger { get { return _logger; } }
        
        public BaseClient(ILogger logger, LotusApi api)
        {
            _logger = logger;
            _api = api;
        }

        protected async Task<ResponseBase<T>> ExecuteAsync<T>(RequestBase model)
        {
            var client = new RestClient(_api.Api);
            client.Timeout = model.Timeout ?? _api.Timeout;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + _api.Token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                _logger.LogError(0, "{method} fail:", model.Method);
                if (!string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogError(0, "content: {content}", response.Content);
                }
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _logger.LogError(0, "error message: {message}", response.ErrorMessage);
                }
                if (response.StatusCode > 0)
                {
                    _logger.LogError(0, "status code: {code}", response.StatusCode);
                }
                return ResponseBase<T>.Fail((int)response.StatusCode, response.StatusDescription);
            }
            var data = JsonConvert.DeserializeObject<ResponseBase<T>>(response.Content);
            if (!data.Success)
            {
                _logger.LogError(0, "{method} code:{code}, message:{message}", model.Method, data.Error.Code, data.Error.Message);
            }
            if (data.Result == null || model.Debug)
            {
                _logger.LogWarning(0, "{method} Content:{message}", model.Method, response.Content);
            }

            return data;
        }

        protected async Task<ResponseBase> ExecuteAsync(RequestBase model)
        {
            return await ExecuteAsync<string>(model);
        }
    }
}
