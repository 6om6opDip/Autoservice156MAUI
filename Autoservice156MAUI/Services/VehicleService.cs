using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services;

public class VehicleService : IVehicleService
{
    private readonly IApiService _apiService;

    public VehicleService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        try
        {
            return await _apiService.GetAsync<IEnumerable<VehicleDto>>("vehicles");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting vehicles: {ex.Message}");
            return Enumerable.Empty<VehicleDto>();
        }
    }

    public async Task<VehicleDto> GetVehicleByIdAsync(int id)
    {
        try
        {
            return await _apiService.GetAsync<VehicleDto>($"vehicles/{id}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting vehicle {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicle)
    {
        try
        {
            return await _apiService.PostAsync<VehicleDto>("vehicles", vehicle);
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error creating vehicle: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateVehicleAsync(int id, VehicleDto vehicle)
    {
        try
        {
            await _apiService.PutAsync<VehicleDto>($"vehicles/{id}", vehicle);
            return true;
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error updating vehicle {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        try
        {
            return await _apiService.DeleteAsync($"vehicles/{id}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error deleting vehicle {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesByClientIdAsync(int clientId)
    {
        try
        {
            return await _apiService.GetAsync<IEnumerable<VehicleDto>>($"vehicles/client/{clientId}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting vehicles for client {clientId}: {ex.Message}");
            return Enumerable.Empty<VehicleDto>();
        }
    }
}