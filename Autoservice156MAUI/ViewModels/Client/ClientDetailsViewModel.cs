using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Client;
using Autoservice156MAUI.Views.Vehicle;

namespace Autoservice156MAUI.ViewModels.Client;

public class ClientDetailsViewModel : BaseViewModel
{
    private readonly IClientService _clientService;
    private readonly IVehicleService _vehicleService;
    private int _clientId;
    private ClientDto _client;
    private VehicleDto _selectedVehicle;

    public ClientDto Client
    {
        get => _client;
        set => SetProperty(ref _client, value);
    }

    public ObservableCollection<VehicleDto> Vehicles { get; } = new();

    public VehicleDto SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            SetProperty(ref _selectedVehicle, value);
            if (value != null)
            {
                SelectVehicle(value);
            }
        }
    }

    public ICommand LoadClientCommand { get; }
    public ICommand EditClientCommand { get; }
    public ICommand DeleteClientCommand { get; }
    public ICommand AddVehicleCommand { get; }
    public ICommand RefreshCommand { get; }

    public ClientDetailsViewModel(IClientService clientService, IVehicleService vehicleService)
    {
        _clientService = clientService;
        _vehicleService = vehicleService;
        Title = "Информация о клиенте";

        LoadClientCommand = new Command(async () => await LoadClientAsync());
        EditClientCommand = new Command(async () => await EditClientAsync());
        DeleteClientCommand = new Command(async () => await DeleteClientAsync());
        AddVehicleCommand = new Command(async () => await AddVehicleAsync());
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    public void SetClientId(int clientId)
    {
        _clientId = clientId;
        LoadClientCommand.Execute(null);
    }

    private async Task LoadClientAsync()
    {
        if (IsBusy || _clientId <= 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // Загружаем клиента
            Client = await _clientService.GetClientByIdAsync(_clientId);
            Title = Client.FullName;

            // Загружаем транспорт клиента
            var vehicles = await _clientService.GetClientVehiclesAsync(_clientId);

            Vehicles.Clear();
            foreach (var vehicle in vehicles)
            {
                Vehicles.Add(vehicle);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task EditClientAsync()
    {
        if (Client == null)
            return;

        var parameters = new Dictionary<string, object>
        {
            { "Client", Client },
            { "IsEditMode", true }
        };
        await NavigateToAsync(nameof(ClientEditPage), parameters);
    }

    private async Task DeleteClientAsync()
    {
        if (Client == null)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Подтверждение",
            $"Вы уверены, что хотите удалить клиента {Client.FullName}?",
            "Удалить",
            "Отмена");

        if (confirm)
        {
            try
            {
                IsBusy = true;
                var success = await _clientService.DeleteClientAsync(Client.Id);

                if (success)
                {
                    await DisplayAlert("Успех", "Клиент удален", "OK");
                    await GoBackAsync();
                }
                else
                {
                    ErrorMessage = "Не удалось удалить клиента";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка удаления: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    private async Task AddVehicleAsync()
    {
        await DisplayAlert("Добавление транспорта",
               "Функция будет доступна после создания страницы", "OK");
    }

    private async void SelectVehicle(VehicleDto vehicle)
    {
        var parameters = new Dictionary<string, object>
        {
            { "VehicleId", vehicle.Id }
        };
        await NavigateToAsync(nameof(VehicleDetailsPage), parameters);
        SelectedVehicle = null;
    }

    private async Task RefreshAsync()
    {
        await LoadClientAsync();
    }
}