using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using Autoservice156MAUI.Views.Vehicle;
using System.Text.Json;

namespace Autoservice156MAUI.ViewModels.Vehicle
{
    public class VehicleListViewModel : BaseViewModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly ApiService _apiService;
        private VehicleDto _selectedVehicle;
        private string _searchText = string.Empty;
        private bool _isApiConnected;
        private string _apiStatusText = "Проверка подключения...";

        private readonly bool _developmentMode = true;

        public ObservableCollection<VehicleDto> Vehicles { get; } = new();
        public ObservableCollection<VehicleDto> FilteredVehicles { get; } = new();

        // Свойства для статуса API
        public bool IsApiConnected
        {
            get => _isApiConnected;
            set => SetProperty(ref _isApiConnected, value);
        }

        public string ApiStatusText
        {
            get => _apiStatusText;
            set => SetProperty(ref _apiStatusText, value);
        }

        public Color ApiStatusColor => IsApiConnected
            ? Color.FromArgb("#4CAF50")  // Зеленый
            : Color.FromArgb("#F44336"); // Красный

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

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterVehicles();
                }
            }
        }

        public ICommand LoadVehiclesCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CheckApiCommand { get; }
        public ICommand TestApiDirectCommand { get; }

        public VehicleListViewModel(IVehicleService vehicleService, ApiService apiService)
        {
            _vehicleService = vehicleService;
            _apiService = apiService;
            Title = "Транспортные средства";

            // Инициализируем команды
            LoadVehiclesCommand = new Command(async () => await LoadVehiclesAsync());
            AddVehicleCommand = new Command(async () => await AddVehicleAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());
            CheckApiCommand = new Command(async () => await CheckApiConnectionAsync());
            TestApiDirectCommand = new Command(async () => await TestApiDirectlyAsync());

            // При запуске проверяем API и загружаем данные
            Task.Run(async () =>
            {
                await CheckApiConnectionAsync();
                await LoadLocalVehiclesAsync(); // Загружаем локальные данные сразу
            });
        }

        public async Task CheckApiConnectionAsync()
        {
            if (IsBusy) return;

            try
            {
                if (_developmentMode)
                {
                    IsApiConnected = true;
                    ApiStatusText = "API подключено (режим разработки)";
                    return;
                }

                IsApiConnected = await _apiService.TestConnectionAsync();
                ApiStatusText = IsApiConnected ? "API подключено" : "API недоступно";
            }
            catch (Exception ex)
            {
                IsApiConnected = false;
                ApiStatusText = $"Ошибка: {ex.Message}";
                Console.WriteLine($"❌ Ошибка проверки API: {ex.Message}");
            }
        }

        public async Task LoadVehiclesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                await LoadLocalVehiclesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки транспорта: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadLocalVehiclesAsync()
        {
            try
            {
                Vehicles.Clear();

                var vehiclesJson = Preferences.Default.Get("local_vehicles", "[]");

                if (!string.IsNullOrEmpty(vehiclesJson) && vehiclesJson != "[]")
                {
                    try
                    {
                        var localVehicles = JsonSerializer.Deserialize<List<VehicleDto>>(vehiclesJson);

                        if (localVehicles != null && localVehicles.Any())
                        {
                            var uniqueVehicles = localVehicles
                                .GroupBy(v => $"{v.Brand}_{v.Model}_{v.LicensePlate}")
                                .Select(g => g.First())
                                .ToList();

                            foreach (var vehicle in uniqueVehicles)
                            {
                                Vehicles.Add(vehicle);
                            }
                        }
                    }
                    catch (JsonException) { }
                }

                FilterVehicles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки локальных данных: {ex.Message}");
            }
        }

        private void FilterVehicles()
        {
            FilteredVehicles.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var vehicle in Vehicles)
                {
                    FilteredVehicles.Add(vehicle);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var vehicle in Vehicles.Where(v =>
                    v.FullName.ToLower().Contains(searchLower) ||
                    v.LicensePlate.ToLower().Contains(searchLower) ||
                    v.Brand.ToLower().Contains(searchLower) ||
                    v.Model.ToLower().Contains(searchLower)))
                {
                    FilteredVehicles.Add(vehicle);
                }
            }
        }

        private async void SelectVehicle(VehicleDto vehicle)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Транспорт",
                    $"{vehicle.FullName}\n" +
                    $"Госномер: {vehicle.LicensePlate}\n" +
                    $"VIN: {vehicle.VIN}\n" +
                    $"Год: {vehicle.Year}",
                    "OK");

                SelectedVehicle = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка выбора транспорта: {ex.Message}");
            }
        }


        private async Task AddVehicleAsync()
        {
            try
            {
                Console.WriteLine("🚗 Кнопка 'Добавить транспорт' нажата (из ViewModel)");

                // Используем метод навигации из BaseViewModel
                await NavigateToAsync($"//VehicleEditPage");

                Console.WriteLine("✅ Навигация выполнена");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Ошибка навигации: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    $"Не удалось открыть форму: {ex.Message}",
                    "OK");
            }
        }

        private async Task RefreshAsync()
        {
            await LoadVehiclesAsync();
        }

        private async Task TestApiDirectlyAsync()
        {
            try
            {
                IsBusy = true;

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                var testUrl = "http://localhost:5136/api/Auth/test";
                Console.WriteLine($"🔗 Тестируем: {testUrl}");

                try
                {
                    var response = await client.GetAsync(testUrl);
                    var content = await response.Content.ReadAsStringAsync();

                    await Application.Current.MainPage.DisplayAlert(
                        "Тест API",
                        $"URL: {testUrl}\n" +
                        $"Статус: {response.StatusCode}\n" +
                        $"Ответ: {content}\n\n" +
                        $"IsSuccessStatusCode: {response.IsSuccessStatusCode}",
                        "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка теста",
                        $"URL: {testUrl}\n" +
                        $"Ошибка: {ex.Message}\n" +
                        $"Тип: {ex.GetType().Name}",
                        "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void OnAppearing()
        {
            // В режиме разработки сразу показываем "API подключено"
            if (_developmentMode)
            {
                IsApiConnected = true;
                ApiStatusText = "API подключено (режим разработки)";
            }

            // Обновляем список при каждом открытии страницы
            await LoadVehiclesAsync();
        }
    }
}