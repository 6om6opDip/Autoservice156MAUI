using Autoservice156MAUI.Models.DTO.Auth;

namespace Autoservice156MAUI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<bool> ValidateTokenAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    string CurrentToken { get; }
    string CurrentUserEmail { get; }
}