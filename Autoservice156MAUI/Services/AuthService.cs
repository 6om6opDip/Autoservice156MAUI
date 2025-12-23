using System.Text.Json;
using Autoservice156MAUI.Models.DTO.Auth;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private const string TokenKey = "autoservice_auth_token";
    private const string UserEmailKey = "autoservice_user_email";
    private const string UserDataKey = "autoservice_user_data";

    public bool IsAuthenticated { get; private set; }
    public string CurrentToken { get; private set; } = string.Empty;
    public string CurrentUserEmail { get; private set; } = string.Empty;

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
        InitializeFromStorage();
    }

    private async void InitializeFromStorage()
    {
        var token = await SecureStorage.GetAsync(TokenKey);
        var email = await SecureStorage.GetAsync(UserEmailKey);

        if (!string.IsNullOrEmpty(token))
        {
            CurrentToken = token;
            CurrentUserEmail = email ?? string.Empty;
            IsAuthenticated = true;
            _apiService.SetAuthToken(token);
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<AuthResponse>("auth/login", request);

            if (!string.IsNullOrEmpty(response.Token))
            {
                await SaveAuthData(response);
                return response;
            }

            throw new UnauthorizedAccessException("Неверные учетные данные");
        }
        catch (ApiException ex)
        {
            throw new UnauthorizedAccessException("Ошибка авторизации", ex);
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<AuthResponse>("auth/register", request);

            if (!string.IsNullOrEmpty(response.Token))
            {
                await SaveAuthData(response);
                return response;
            }

            throw new InvalidOperationException("Регистрация не удалась");
        }
        catch (ApiException ex)
        {
            throw new InvalidOperationException("Ошибка регистрации", ex);
        }
    }

    public async Task<bool> ValidateTokenAsync()
    {
        if (string.IsNullOrEmpty(CurrentToken))
            return false;

        try
        {
            // Простой запрос для проверки токена
            await _apiService.GetAsync<object>("auth/validate");
            return true;
        }
        catch
        {
            await LogoutAsync();
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        IsAuthenticated = false;
        CurrentToken = string.Empty;
        CurrentUserEmail = string.Empty;

        _apiService.ClearAuthToken();

        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(UserEmailKey);
        SecureStorage.Remove(UserDataKey);
    }

    private async Task SaveAuthData(AuthResponse response)
    {
        CurrentToken = response.Token;
        CurrentUserEmail = response.Email;
        IsAuthenticated = true;

        _apiService.SetAuthToken(response.Token);

        await SecureStorage.SetAsync(TokenKey, response.Token);
        await SecureStorage.SetAsync(UserEmailKey, response.Email);

        // Сохраняем данные пользователя
        var userData = JsonSerializer.Serialize(new
        {
            response.UserId,
            response.FirstName,
            response.LastName,
            response.Role
        });
        await SecureStorage.SetAsync(UserDataKey, userData);
    }
}