using Autoservice156MAUI.Models.DTO.Auth;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using System.Windows.Input;

namespace Autoservice156MAUI.ViewModels.Auth
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _email = "admin@autoservice.com";
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = "admin123";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;

            LoginCommand = new Command(async () => await LoginAsync());
            NavigateToRegisterCommand = new Command(async () =>
                await Shell.Current.GoToAsync("//RegisterPage"));
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    "Введите email и пароль",
                    "OK");
                return;
            }

            IsBusy = true;

            try
            {
                Console.WriteLine($"🔐 Начало входа...");

                var loginRequest = new LoginRequest
                {
                    Email = Email,
                    Password = Password
                };

                var authResponse = await _authService.LoginAsync(loginRequest);

                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    Console.WriteLine($"✅ Вход успешен! Переход к списку клиентов...");

                    // Переход на главную страницу
                    await Shell.Current.GoToAsync("//ClientListPage");

                    // Очистка полей
                    Email = string.Empty;
                    Password = string.Empty;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка",
                        "Неверный email или пароль",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");

                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    $"Не удалось войти: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}