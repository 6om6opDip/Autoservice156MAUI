namespace Autoservice156MAUI.Views.Auth;

public partial class LoginPage : ContentPage
{
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public LoginPage()
    {
        InitializeComponent();

        // Автозаполнение для теста на Windows
        EmailEntry.Text = "admin@example.com";
        PasswordEntry.Text = "admin123";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (IsLoading) return;

        // Валидация
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            ShowError("Введите email");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ShowError("Введите пароль");
            return;
        }

        IsLoading = true;

        try
        {
            // Имитация загрузки (на Windows можно без задержки)
            await Task.Delay(500); // Удали эту строку для реального API

            // TODO: Заменить на реальный вызов API
            // var authService = ... // Получить из DI
            // var result = await authService.LoginAsync(...);

            // Временная заглушка - всегда успех для теста
            bool loginSuccess = true;

            if (loginSuccess)
            {
                // Успешный вход - переходим на главную
                await Shell.Current.GoToAsync("///MainTabBar");
            }
            else
            {
                ShowError("Неверный email или пароль");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SetLoading(bool loading)
    {
        IsLoading = loading;

        // Блокируем UI при загрузке
        EmailEntry.IsEnabled = !loading;
        PasswordEntry.IsEnabled = !loading;
        RememberMeCheckbox.IsEnabled = !loading;
    }

    private void ShowError(string message)
    {
        ErrorMessageLabel.Text = message;
        ErrorMessageLabel.IsVisible = true;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Регистрация",
            "Обратитесь к администратору для создания учетной записи", "OK");
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Восстановление пароля",
            "Свяжитесь с системным администратором", "OK");
    }
}