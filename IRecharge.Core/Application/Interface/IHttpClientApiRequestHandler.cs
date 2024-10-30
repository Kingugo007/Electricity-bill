
namespace IRecharge.Core.Application.Interface
{
    public interface IHttpClientApiRequestHandler
    {
        Task<TResponse> DeleteAsync<TResponse>(string url);
        Task<TResponse> GetAsync<TResponse>(string url);
        Task<TResponse> PostAsync<TResponse>(string url, object body);
        Task<TResponse> PutAsync<TResponse>(string url, object body);
        Task<TResponse> SendRequestAsync<TResponse>(string url, HttpMethod method, object body = null);
    }
}