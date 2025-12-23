using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Client;

namespace Autoservice156MAUI.ViewModels.Client;

public class ClientEditViewModel : BaseViewModel
{
    private readonly IClientService _clientService;
    private ClientDto _client;
    private bool _isEditMode;

    public ClientDto Client
    {
        get => _client;
        set => SetProperty(ref _client, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ClientEditViewModel(IClientService clientService)
    {
        _clientService = clientService;
        Title = "Новый клиент";
        Client = new ClientDto();

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());
    }

    public void SetClient(ClientDto client, bool isEditMode = false)
    {
        Client = client ?? new ClientDto();
        IsEditMode = isEditMode;
        Title = isEditMode ? "Редактирование клиента" : "Новый клиент";
    }

    public void SetClientId(int clientId)
    {
        // Для создания нового клиента с привязкой к родителю
        Client = new ClientDto();
        // Здесь можно установить дополнительные параметры если нужно
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        // Валидация
        if (string.IsNullOrWhiteSpace(Client.FirstName))
        {
            ErrorMessage = "Введите имя";
            return;
        }

        if (string.IsNullOrWhiteSpace(Client.LastName))
        {
            ErrorMessage = "Введите фамилию";
            return;
        }

        if (string.IsNullOrWhiteSpace(Client.Email))
        {
            ErrorMessage = "Введите email";
            return;
        }

        if (string.IsNullOrWhiteSpace(Client.Phone))
        {
            ErrorMessage = "Введите телефон";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (IsEditMode && Client.Id > 0)
            {
                // Обновление существующего клиента
                var success = await _clientService.UpdateClientAsync(Client.Id, Client);
                if (success)
                {
                    await DisplayAlert("Успех", "Данные клиента обновлены", "OK");
                    await GoBackAsync();
                }
                else
                {
                    ErrorMessage = "Не удалось обновить данные";
                }
            }
            else
            {
                // Создание нового клиента
                var newClient = await _clientService.CreateClientAsync(Client);
                await DisplayAlert("Успех", "Клиент создан", "OK");

                // Возвращаемся назад или переходим к деталям
                var parameters = new Dictionary<string, object>
                {
                    { "ClientId", newClient.Id }
                };
                await NavigateToAsync($"../{nameof(ClientDetailsPage)}", parameters);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка сохранения: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CancelAsync()
    {
        bool confirm = true;

        if (!string.IsNullOrWhiteSpace(Client.FirstName) ||
            !string.IsNullOrWhiteSpace(Client.LastName))
        {
            confirm = await Application.Current.MainPage.DisplayAlert(
                "Подтверждение",
                "Отменить ввод? Все несохраненные данные будут потеряны.",
                "Да",
                "Нет");
        }

        if (confirm)
        {
            await GoBackAsync();
        }
    }
}