namespace Autoservice156MAUI.Views.Auth
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            // Для теста
            EmailEntry.Text = "test@example.com";
            PasswordEntry.Text = "Test123!";
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
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

            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            ErrorMessageLabel.IsVisible = false;

            try
            {
                // Симуляция загрузки
                await Task.Delay(1500);

                // TODO: Реальная аутентификация
                if (EmailEntry.Text == "admin@example.com" && PasswordEntry.Text == "admin")
                {
                    // Успешный вход
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
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Регистрация", "Функция регистрации будет доступна позже", "OK");
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Восстановление пароля",
                "Обратитесь к администратору", "OK");
        }

        private void ShowError(string message)
        {
            ErrorMessageLabel.Text = message;
            ErrorMessageLabel.IsVisible = true;
        }
    }
}