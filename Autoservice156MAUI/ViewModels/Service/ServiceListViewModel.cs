using System.Collections.ObjectModel;
using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;

namespace Autoservice156MAUI.ViewModels.Service;

public class ServiceListViewModel : BaseViewModel
{
    private readonly IServiceService _serviceService;
    private ServiceDto _selectedService;
    private string _searchText = string.Empty;

    public ObservableCollection<ServiceDto> Services { get; } = new();
    public ObservableCollection<ServiceDto> FilteredServices { get; } = new();

    public ServiceDto SelectedService
    {
        get => _selectedService;
        set
        {
            SetProperty(ref _selectedService, value);
            if (value != null)
            {
                SelectService(value);
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
                FilterServices();
            }
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
    }

    public async Task LoadServicesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var services = await _serviceService.GetAllServicesAsync();

            Services.Clear();
            foreach (var service in services)
            {
                Services.Add(service);
            }

            FilterServices();
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

    private void FilterServices()
    {
        FilteredServices.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var service in Services)
            {
                FilteredServices.Add(service);
            }
        }
        else
        {
            var searchLower = SearchText.ToLower();
            foreach (var service in Services.Where(s =>
                s.Name.ToLower().Contains(searchLower) ||
                s.Description.ToLower().Contains(searchLower) ||
                s.Category.ToLower().Contains(searchLower)))
            {
                FilteredServices.Add(service);
            }
        }
    }

    private async void SelectService(ServiceDto service)
    {
        await DisplayAlert(service.Name,
            $"Цена: {service.Price}₽\nДлительность: {service.DurationMinutes} мин.\n\n{service.Description}",
            "OK");
        SelectedService = null;
    }

    private async Task AddServiceAsync()
    {
        await DisplayAlert("Добавление услуги", "Функция будет доступна позже", "OK");
    }

    private async Task RefreshAsync()
    {
        await LoadServicesAsync();
    }

    public void OnAppearing()
    {
        if (!Services.Any())
        {
            LoadServicesCommand.Execute(null);
        }
    }
}