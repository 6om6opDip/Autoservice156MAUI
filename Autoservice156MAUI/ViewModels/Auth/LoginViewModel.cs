using System.Windows.Input;
using Autoservice156MAUI.Models.DTO.Auth;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Auth;

namespace Autoservice156MAUI.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _rememberMe = false;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand ForgotPasswordCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Вход в систему";

        // Для теста
        Email = "test@example.com";
        Password = "Test123!";

        LoginCommand = new Command(async () => await LoginAsync());
        RegisterCommand = new Command(async () => await RegisterAsync());
        ForgotPasswordCommand = new Command(async () => await ForgotPasswordAsync());
    }

    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Введите email";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Введите пароль";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var request = new LoginRequest
            {
                Email = Email,
                Password = Password
            };

            var response = await _authService.LoginAsync(request);

            // Успешный вход
            await DisplayAlert("Успех", "Вход выполнен успешно!", "OK");
            await NavigateToAsync("///MainTabBar");
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = "Неверный email или пароль";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка входа: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RegisterAsync()
    {
        await NavigateToAsync(nameof(RegisterPage));
    }

    private async Task ForgotPasswordAsync()
    {
        await DisplayAlert("Восстановление пароля",
            "Для восстановления пароля обратитесь к администратору", "OK");
    }
}