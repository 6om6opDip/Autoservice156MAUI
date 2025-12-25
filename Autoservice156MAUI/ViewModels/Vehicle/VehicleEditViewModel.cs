using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Vehicle;

namespace Autoservice156MAUI.ViewModels.Vehicle
{
    public class VehicleEditViewModel : BaseViewModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IClientService _clientService;
        private VehicleDto _vehicle;
        private bool _isEditMode;
        private long? _clientId;

        public VehicleDto Vehicle
        {
            get => _vehicle;
            set => SetProperty(ref _vehicle, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ObservableCollection<ClientDto> Clients { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadClientsCommand { get; }

        public VehicleEditViewModel(IVehicleService vehicleService, IClientService clientService)
        {
            _vehicleService = vehicleService;
            _clientService = clientService;
            Title = "Новый транспорт";
            Vehicle = new VehicleDto();

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await CancelAsync());
            LoadClientsCommand = new Command(async () => await LoadClientsAsync());
        }

        public void SetVehicle(VehicleDto vehicle, bool isEditMode = false)
        {
            Vehicle = vehicle ?? new VehicleDto();
            IsEditMode = isEditMode;
            Title = isEditMode ? "Редактирование транспорта" : "Новый транспорт";
            LoadClientsCommand.Execute(null);
        }

        public void SetClientId(long clientId)
        {
            _clientId = clientId;
            if (Vehicle != null)
            {
                Vehicle.ClientId = clientId;
            }
            LoadClientsCommand.Execute(null);
        }

        private async Task LoadClientsAsync()
        {
            if (IsBusy) return;

            try
            {
                var clients = await _clientService.GetAllClientsAsync();

                Clients.Clear();
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }

                if (_clientId.HasValue && _clientId.Value > 0)
                {
                    var client = Clients.FirstOrDefault(c => c.Id == _clientId.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Vehicle.Brand))
            {
                ErrorMessage = "Введите марку";
                return;
            }

            if (string.IsNullOrWhiteSpace(Vehicle.Model))
            {
                ErrorMessage = "Введите модель";
                return;
            }

            if (Vehicle.Year < 1900 || Vehicle.Year > DateTime.Now.Year + 1)
            {
                ErrorMessage = "Введите корректный год";
                return;
            }

            if (string.IsNullOrWhiteSpace(Vehicle.LicensePlate))
            {
                ErrorMessage = "Введите номерной знак";
                return;
            }

            if (Vehicle.ClientId <= 0)
            {
                ErrorMessage = "Выберите владельца";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (IsEditMode && Vehicle.Id > 0)
                {
                    var success = await _vehicleService.UpdateVehicleAsync((int)Vehicle.Id, Vehicle);
                    if (success)
                    {
                        await DisplayAlert("Успех", "Данные транспорта обновлены", "OK");
                        await GoBackAsync();
                    }
                    else
                    {
                        ErrorMessage = "Не удалось обновить данные";
                    }
                }
                else
                {
                    var newVehicle = await _vehicleService.CreateVehicleAsync(Vehicle);
                    await DisplayAlert("Успех", "Транспорт создан", "OK");

                    var parameters = new Dictionary<string, object>
                    {
                        { "VehicleId", newVehicle.Id }
                    };
                    await NavigateToAsync($"../{nameof(VehicleDetailsPage)}", parameters);
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

            if (!string.IsNullOrWhiteSpace(Vehicle.Brand) ||
                !string.IsNullOrWhiteSpace(Vehicle.Model))
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
}