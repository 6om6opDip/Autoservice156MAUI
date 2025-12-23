using System.Windows.Input;
using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Base;

namespace Autoservice156MAUI.ViewModels.Service;

public class ServiceDetailsViewModel : BaseViewModel
{
    private readonly IServiceService _serviceService;
    private int _serviceId;
    private ServiceDto _service;

    public ServiceDto Service
    {
        get => _service;
        set => SetProperty(ref _service, value);
    }

    public ICommand LoadServiceCommand { get; }
    public ICommand BookServiceCommand { get; }

    public ServiceDetailsViewModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
        Title = "Детали услуги";

        LoadServiceCommand = new Command(async () => await LoadServiceAsync());
        BookServiceCommand = new Command(async () => await BookServiceAsync());
    }

    public void SetServiceId(int serviceId)
    {
        _serviceId = serviceId;
        LoadServiceCommand.Execute(null);
    }

    private async Task LoadServiceAsync()
    {
        if (IsBusy || _serviceId <= 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            Service = await _serviceService.GetServiceByIdAsync(_serviceId);
            Title = Service.Name;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки услуги: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BookServiceAsync()
    {
        await DisplayAlert("Запись на услугу", "Функция записи будет доступна позже", "OK");
    }
}