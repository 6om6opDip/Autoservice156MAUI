using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string DefaultBaseUrl = "http://10.0.2.2:5000/api/"; // Android эмулятор

    public bool HasToken { get; private set; }

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(DefaultBaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public void SetAuthToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            ClearAuthToken();
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        HasToken = true;
    }

    public void ClearAuthToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        HasToken = false;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"GET запрос не удался: {ex.Message}", ex);
        }
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"POST запрос не удался: {ex.Message}", ex);
        }
    }

    public async Task<T> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"PUT запрос не удался: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

public class ApiException : Exception
{
    public ApiException(string message, Exception innerException)
        : base(message, innerException) { }
}