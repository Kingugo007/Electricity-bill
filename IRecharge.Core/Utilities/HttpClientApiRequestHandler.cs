using IRecharge.Core.Application.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace IRecharge.Core.Application
{
    public class HttpClientApiRequestHandler : IHttpClientApiRequestHandler
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientApiRequestHandler> _logger;
        public HttpClientApiRequestHandler(ILogger<HttpClientApiRequestHandler> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<TResponse> SendRequestAsync<TResponse>(string url, HttpMethod method, object body = null)
        {

            var requestMessage = new HttpRequestMessage(method, url);
            if (body != null && (method == HttpMethod.Post || method == HttpMethod.Put))
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                requestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Request: {response.Content}");
                    throw new ArgumentException($"Error: {response.ReasonPhrase}, Status Code: {(int)response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(jsonResponse);

            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Request failed: {ex.Message}", ex);

            }

        }

        public Task<TResponse> GetAsync<TResponse>(string url)

            => SendRequestAsync<TResponse>(url, HttpMethod.Get);

        public Task<TResponse> PostAsync<TResponse>(string url, object body)

            => SendRequestAsync<TResponse>(url, HttpMethod.Post, body);

        public Task<TResponse> PutAsync<TResponse>(string url, object body)

            => SendRequestAsync<TResponse>(url, HttpMethod.Put, body);

        public Task<TResponse> DeleteAsync<TResponse>(string url)

            => SendRequestAsync<TResponse>(url, HttpMethod.Delete);

    }

}
