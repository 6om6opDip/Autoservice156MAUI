using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services;

public class ServiceService : IServiceService
{
    private readonly IApiService _apiService;

    public ServiceService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        try
        {
            return await _apiService.GetAsync<IEnumerable<ServiceDto>>("services");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting services: {ex.Message}");
            return Enumerable.Empty<ServiceDto>();
        }
    }

    public async Task<ServiceDto> GetServiceByIdAsync(int id)
    {
        try
        {
            return await _apiService.GetAsync<ServiceDto>($"services/{id}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting service {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<ServiceDto> CreateServiceAsync(ServiceDto service)
    {
        try
        {
            return await _apiService.PostAsync<ServiceDto>("services", service);
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error creating service: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateServiceAsync(int id, ServiceDto service)
    {
        try
        {
            await _apiService.PutAsync<ServiceDto>($"services/{id}", service);
            return true;
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error updating service {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        try
        {
            return await _apiService.DeleteAsync($"services/{id}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error deleting service {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<ServiceDto>> GetActiveServicesAsync()
    {
        try
        {
            var allServices = await GetAllServicesAsync();
            return allServices.Where(s => s.IsActive);
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting active services: {ex.Message}");
            return Enumerable.Empty<ServiceDto>();
        }
    }
}