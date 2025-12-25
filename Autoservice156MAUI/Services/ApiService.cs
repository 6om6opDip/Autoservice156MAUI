using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl = "http://localhost:5136/api/";
        private readonly JsonSerializerOptions _jsonOptions;
        private string _currentToken;

        public bool HasToken => !string.IsNullOrEmpty(_currentToken);

        public ApiService()
        {

            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Загружаем токен
            LoadTokenAsync().Wait();
        }

        private async Task LoadTokenAsync()
        {
            try
            {
                _currentToken = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(_currentToken))
                {
                    SetAuthToken(_currentToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки токена: {ex.Message}");
            }
        }

        public void SetAuthToken(string token)
        {
            _currentToken = token;

            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Сохраняем
            SecureStorage.Default.SetAsync("auth_token", token);
        }

        public void ClearAuthToken()
        {
            _currentToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            SecureStorage.Default.Remove("auth_token");
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                Console.WriteLine($"🔗 GET запрос: {_baseUrl}{endpoint}");

                var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");

                await HandleResponse(response);

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Ответ получен: {json.Length} символов");

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка GET: {ex.Message}");
                throw new ApiException($"Ошибка получения данных: {ex.Message}");
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                Console.WriteLine($"🔗 POST запрос: {_baseUrl}{endpoint}");

                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);

                await HandleResponse(response);

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Ответ: {responseJson}");

                return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка POST: {ex.Message}");
                throw new ApiException($"Ошибка отправки данных: {ex.Message}");
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                Console.WriteLine($"🔗 PUT запрос: {_baseUrl}{endpoint}");

                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}{endpoint}", content);

                await HandleResponse(response);

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Ответ: {responseJson}");

                return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка PUT: {ex.Message}");
                throw new ApiException($"Ошибка обновления данных: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                Console.WriteLine($"🔗 DELETE запрос: {_baseUrl}{endpoint}");

                var response = await _httpClient.DeleteAsync($"{_baseUrl}{endpoint}");

                await HandleResponse(response);

                Console.WriteLine($"✅ Удаление успешно");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка DELETE: {ex.Message}");
                throw new ApiException($"Ошибка удаления: {ex.Message}");
            }
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("❌ 401 Unauthorized - токен недействителен");

                ClearAuthToken();

                throw new UnauthorizedAccessException("Сессия истекла. Пожалуйста, войдите снова.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ HTTP ошибка {(int)response.StatusCode}: {errorContent}");

                throw new ApiException($"HTTP ошибка {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                Console.WriteLine($"🔗 Проверка API: {_baseUrl}Auth/test");

                // Создаем запрос БЕЗ авторизационного заголовка
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}Auth/test");

                // Временно удаляем токен для тестового запроса
                var originalAuth = _httpClient.DefaultRequestHeaders.Authorization;
                _httpClient.DefaultRequestHeaders.Authorization = null;

                try
                {
                    var response = await _httpClient.SendAsync(request);

                    Console.WriteLine($"📡 Статус: {response.StatusCode} ({(int)response.StatusCode})");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"✅ API ответило: {content}");
                        return true;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("⚠️ 401 Unauthorized: Добавьте [AllowAnonymous] к Auth/test в API");
                        return false;
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"❌ Ошибка API: {error}");
                        return false;
                    }
                }
                finally
                {
                    // Восстанавливаем оригинальный токен
                    _httpClient.DefaultRequestHeaders.Authorization = originalAuth;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Ошибка HTTP: {ex.Message}");
                Console.WriteLine($"📌 Проверьте:\n1. API запущен на localhost:5136\n2. CORS настроен\n3. Брандмауэр не блокирует порт");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Ошибка: {ex.Message}");
                return false;
            }
        }
    }

    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }
}





