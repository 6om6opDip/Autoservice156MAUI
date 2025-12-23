using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Client;

namespace Autoservice156MAUI.ViewModels.Vehicle;

public class VehicleDetailsViewModel : BaseViewModel
{
    private readonly IVehicleService _vehicleService;
    private readonly IClientService _clientService;
    private int _vehicleId;
    private VehicleDto _vehicle;
    private ClientDto _client;

    public VehicleDto Vehicle
    {
        get => _vehicle;
        set => SetProperty(ref _vehicle, value);
    }

    public ClientDto Client
    {
        get => _client;
        set => SetProperty(ref _client, value);
    }

    public ICommand LoadVehicleCommand { get; }
    public ICommand EditVehicleCommand { get; }
    public ICommand DeleteVehicleCommand { get; }
    public ICommand ViewClientCommand { get; }
    public ICommand RefreshCommand { get; }

    public VehicleDetailsViewModel(IVehicleService vehicleService, IClientService clientService)
    {
        _vehicleService = vehicleService;
        _clientService = clientService;
        Title = "Информация о транспорте";

        LoadVehicleCommand = new Command(async () => await LoadVehicleAsync());
        EditVehicleCommand = new Command(async () => await EditVehicleAsync());
        DeleteVehicleCommand = new Command(async () => await DeleteVehicleAsync());
        ViewClientCommand = new Command(async () => await ViewClientAsync());
        RefreshCommand = new Command(async () => await RefreshAsync());
    }

    public void SetVehicleId(int vehicleId)
    {
        _vehicleId = vehicleId;
        LoadVehicleCommand.Execute(null);
    }

    private async Task LoadVehicleAsync()
    {
        if (IsBusy || _vehicleId <= 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // Загружаем транспорт
            Vehicle = await _vehicleService.GetVehicleByIdAsync(_vehicleId);
            Title = Vehicle.FullName;

            // Загружаем владельца
            if (Vehicle.ClientId > 0)
            {
                Client = await _clientService.GetClientByIdAsync(Vehicle.ClientId);
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


    private async Task EditVehicleAsync()
    {
        if (Vehicle == null)
            return;

        // Временно используем заглушку
        await DisplayAlert("Редактирование",
            "Функция редактирования транспорта будет доступна позже", "OK");

        // Когда создашь страницу, раскомментируй:
        // var parameters = new Dictionary<string, object>
        // {
        //     { "Vehicle", Vehicle },
        //     { "IsEditMode", true }
        // };
        // await NavigateToAsync("VehicleEditPage", parameters);
    }

    private async Task DeleteVehicleAsync()
    {
        if (Vehicle == null)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Подтверждение",
            $"Вы уверены, что хотите удалить транспорт {Vehicle.FullName}?",
            "Удалить",
            "Отмена");

        if (confirm)
        {
            try
            {
                IsBusy = true;
                var success = await _vehicleService.DeleteVehicleAsync(Vehicle.Id);

                if (success)
                {
                    await DisplayAlert("Успех", "Транспорт удален", "OK");
                    await GoBackAsync();
                }
                else
                {
                    ErrorMessage = "Не удалось удалить транспорт";
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

    private async Task ViewClientAsync()
    {
        if (Client == null)
            return;

        var parameters = new Dictionary<string, object>
        {
            { "ClientId", Client.Id }
        };
        await NavigateToAsync(nameof(ClientDetailsPage), parameters);
    }

    private async Task RefreshAsync()
    {
        await LoadVehicleAsync();
    }
}