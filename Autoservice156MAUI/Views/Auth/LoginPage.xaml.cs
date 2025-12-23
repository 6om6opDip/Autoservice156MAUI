using Autoservice156MAUI.ViewModels.Auth;

namespace Autoservice156MAUI.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        // Установка тестовых данных
        EmailEntry.Text = "admin@example.com";
        PasswordEntry.Text = "admin";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Показываем индикатор
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        ErrorMessageLabel.IsVisible = false;

        // Блокируем UI
        EmailEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;
        RememberMeCheckbox.IsEnabled = false;

        try
        {
            // Задержка для имитации сети
            await Task.Delay(800);

            // Простая проверка (замени на реальную аутентизацию)
            if (EmailEntry.Text == "admin@example.com" && PasswordEntry.Text == "admin")
            {
                // Успешный вход
                await DisplayAlert("Успех", "Вход выполнен!", "OK");

                // Переходим на главную страницу
                await Shell.Current.GoToAsync("///MainPage");
            }
            else if (EmailEntry.Text == "user@example.com" && PasswordEntry.Text == "user123")
            {
                // Другой тестовый пользователь
                await DisplayAlert("Успех", "Вход выполнен!", "OK");
                await Shell.Current.GoToAsync("///MainPage");
            }
            else
            {
                // Ошибка входа
                ErrorMessageLabel.Text = "Неверный email или пароль";
                ErrorMessageLabel.IsVisible = true;

                // Анимация ошибки
                await ErrorMessageLabel.TranslateTo(-10, 0, 50);
                await ErrorMessageLabel.TranslateTo(10, 0, 50);
                await ErrorMessageLabel.TranslateTo(0, 0, 50);
            }
        }
        catch (Exception ex)
        {
            ErrorMessageLabel.Text = $"Ошибка: {ex.Message}";
            ErrorMessageLabel.IsVisible = true;
        }
        finally
        {
            // Восстанавливаем UI
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            EmailEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
            RememberMeCheckbox.IsEnabled = true;
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Регистрация",
            "Для регистрации обратитесь к администратору системы", "OK");
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Восстановление пароля",
            "Обратитесь к системному администратору", "OK");
    }

    // Автозаполнение для тестирования
    private void OnEmailFocused(object sender, FocusEventArgs e)
    {
        // Можно добавить логику при фокусе
    }

    private void OnPasswordFocused(object sender, FocusEventArgs e)
    {
        // Можно добавить логику при фокусе
    }
}
