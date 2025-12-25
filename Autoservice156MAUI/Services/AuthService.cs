using System.Text.Json;
using Autoservice156MAUI.Models.DTO.Auth;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private string _currentUserEmail;
        private AuthResponse _currentAuthResponse;

        public bool IsAuthenticated => !string.IsNullOrEmpty(CurrentToken);
        public string CurrentToken => _currentAuthResponse?.Token;
        public string CurrentUserEmail => _currentUserEmail;

        public AuthService(IApiService apiService)
        {
            _apiService = apiService;
            LoadStoredData();
        }

        private async void LoadStoredData()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("auth_token");
                var email = await SecureStorage.Default.GetAsync("user_email");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
                {
                    _currentUserEmail = email;
                    _currentAuthResponse = new AuthResponse { Token = token };
                    _apiService.SetAuthToken(token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                Console.WriteLine($"🔐 Попытка входа: {loginRequest.Email}");

                var response = await _apiService.PostAsync<AuthResponse>("Auth/login", loginRequest);

                if (!string.IsNullOrEmpty(response?.Token))
                {
                    Console.WriteLine($"✅ Вход успешен, токен получен");

                    _currentAuthResponse = response;
                    _currentUserEmail = loginRequest.Email;
                    _apiService.SetAuthToken(response.Token);

                    // Сохраняем email
                    await SecureStorage.Default.SetAsync("user_email", loginRequest.Email);

                    return response;
                }

                Console.WriteLine($"❌ Вход не удался: токен пустой");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка входа: {ex.Message}");
                throw new Exception($"Ошибка входа: {ex.Message}");
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                Console.WriteLine($"🔐 Регистрация пользователя: {registerRequest.Email}");

                var response = await _apiService.PostAsync<AuthResponse>("Auth/register", registerRequest);

                Console.WriteLine($"✅ Регистрация успешна");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка регистрации: {ex.Message}");
                throw new Exception($"Ошибка регистрации: {ex.Message}");
            }
        }

        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentToken))
                    return false;

                // Можно проверить токен, отправив запрос к защищенному endpoint
                var test = await _apiService.GetAsync<object>("Auth/validate");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            _currentAuthResponse = null;
            _currentUserEmail = null;
            _apiService.ClearAuthToken();

            // Используйте Remove вместо RemoveAsync
            SecureStorage.Default.Remove("auth_token");
            SecureStorage.Default.Remove("user_email");

            Console.WriteLine("✅ Выход выполнен");
        }
    }
}