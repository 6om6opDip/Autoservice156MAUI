using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;
using System.Text.Json;

namespace Autoservice156MAUI.ViewModels.Service
{
    public class ServiceListViewModel : BaseViewModel
    {
        private readonly IServiceService _serviceService;
        private ServiceDto _selectedService;
        private string _searchText = string.Empty;
        private readonly bool _developmentMode = true;

        public ObservableCollection<ServiceDto> Services { get; } = new();
        public ObservableCollection<ServiceDto> FilteredServices { get; } = new();

        public ServiceDto SelectedService
        {
            get => _selectedService;
            set
            {
                SetProperty(ref _selectedService, value);
                if (value != null) SelectService(value);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value)) FilterServices();
            }
        }

        public ICommand LoadServicesCommand { get; }
        public ICommand AddServiceCommand { get; }
        public ICommand RefreshCommand { get; }

        public ServiceListViewModel(IServiceService serviceService)
        {
            _serviceService = serviceService;
            Title = "Услуги";
            LoadServicesCommand = new Command(async () => await LoadServicesAsync());
            AddServiceCommand = new Command(async () => await AddServiceAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());
            Task.Run(async () => await LoadServicesAsync());
        }

        public async Task LoadServicesAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                await LoadLocalServicesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки услуг: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadLocalServicesAsync()
        {
            try
            {
                Services.Clear();

                // 1. Загружаем локальные данные
                var servicesJson = Preferences.Default.Get("local_services", "[]");

                if (!string.IsNullOrEmpty(servicesJson) && servicesJson != "[]")
                {
                    try
                    {
                        var localServices = JsonSerializer.Deserialize<List<ServiceDto>>(servicesJson);
                        if (localServices != null && localServices.Any())
                        {
                            foreach (var service in localServices)
                            {
                                Services.Add(service);
                            }
                        }
                    }
                    catch (JsonException) { }
                }

                // 2. ВСЕГДА добавляем тестовые данные (даже если есть локальные)
                var testServices = new List<ServiceDto>
        {
            new ServiceDto { Id = 1, Name = "Замена масла", Description = "Полная замена моторного масла и масляного фильтра", Price = 2500, DurationMinutes = 60, Category = "Техническое обслуживание", Status = "Active" },
            new ServiceDto { Id = 2, Name = "Диагностика ходовой", Description = "Комплексная диагностика подвески и рулевого управления", Price = 1500, DurationMinutes = 90, Category = "Диагностика", Status = "Active" },
            new ServiceDto { Id = 3, Name = "Замена тормозных колодок", Description = "Замена передних и задних тормозных колодок", Price = 3500, DurationMinutes = 120, Category = "Ремонт тормозов", Status = "Active" },
            new ServiceDto { Id = 4, Name = "Компьютерная диагностика", Description = "Считывание и анализ ошибок электронных систем", Price = 2000, DurationMinutes = 45, Category = "Диагностика", Status = "Active" },
            new ServiceDto { Id = 5, Name = "Замена аккумулятора", Description = "Демонтаж старого и установка нового аккумулятора", Price = 4000, DurationMinutes = 30, Category = "Электрика", Status = "Inactive" },
            new ServiceDto { Id = 6, Name = "Шиномонтаж", Description = "Сезонная замена и балансировка колёс", Price = 3000, DurationMinutes = 120, Category = "Колёсные работы", Status = "Active" }
        };

                // Добавляем тестовые услуги (если их еще нет)
                foreach (var testService in testServices)
                {
                    if (!Services.Any(s => s.Name == testService.Name))
                    {
                        Services.Add(testService);
                    }
                }

                FilterServices();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки локальных услуг: {ex.Message}");
            }
        }

        private void FilterServices()
        {
            FilteredServices.Clear();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var service in Services) FilteredServices.Add(service);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var service in Services.Where(s => s.Name.ToLower().Contains(searchLower) || s.Description.ToLower().Contains(searchLower) || s.Category.ToLower().Contains(searchLower)))
                {
                    FilteredServices.Add(service);
                }
            }
        }

        private async void SelectService(ServiceDto service)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(service.Name, $"Категория: {service.Category}\nЦена: {service.Price}₽\nДлительность: {service.DurationMinutes} мин.\nСтатус: {service.StatusText}\n\nОписание:\n{service.Description}", "OK");
                SelectedService = null;
            }
            catch (Exception) { }
        }

        private async Task AddServiceAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//ServiceEditPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async Task RefreshAsync()
        {
            await LoadServicesAsync();
        }

        public async void OnAppearing()
        {
            await LoadServicesAsync();
        }
    }
}