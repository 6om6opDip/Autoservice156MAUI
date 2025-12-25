using System.Net.Http.Headers;

namespace Autoservice156MAUI.Services.Interfaces;

public interface IApiService
{
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<T> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    void SetAuthToken(string token);
    void ClearAuthToken();
    bool HasToken { get; }
    Task<bool> TestConnectionAsync();
}