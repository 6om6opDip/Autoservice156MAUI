using System.Windows.Input;
using Autoservice156MAUI.Models.DTO.Auth;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;

namespace Autoservice156MAUI.ViewModels.Auth;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _phone = string.Empty;

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

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public ICommand RegisterCommand { get; }
    public ICommand GoBackCommand { get; }

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Регистрация";

        RegisterCommand = new Command(async () => await RegisterAsync());
        GoBackCommand = new Command(async () => await GoBackAsync());
    }

    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        // Валидация
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

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            ErrorMessage = "Введите имя";
            return;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Введите фамилию";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var request = new RegisterRequest
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone
            };

            var response = await _authService.RegisterAsync(request);

            await DisplayAlert("Успех", "Регистрация выполнена успешно!", "OK");
            await GoBackAsync(); // Возвращаемся на страницу входа
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка регистрации: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}