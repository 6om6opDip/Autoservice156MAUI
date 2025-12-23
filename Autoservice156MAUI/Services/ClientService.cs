using Autoservice156MAUI.Models.DTO;
using Autoservice156MAUI.Services.Interfaces;

namespace Autoservice156MAUI.Services;

public class ClientService : IClientService
{
    private readonly IApiService _apiService;

    public ClientService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
    {
        try
        {
            return await _apiService.GetAsync<IEnumerable<ClientDto>>("clients");
        }
        catch (ApiException ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error getting clients: {ex.Message}");
            return Enumerable.Empty<ClientDto>();
        }
    }

    public async Task<ClientDto> GetClientByIdAsync(int id)
    {
        try
        {
            return await _apiService.GetAsync<ClientDto>($"clients/{id}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting client {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<ClientDto> CreateClientAsync(ClientDto client)
    {
        try
        {
            return await _apiService.PostAsync<ClientDto>("clients", client);
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error creating client: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateClientAsync(int id, ClientDto client)
    {
        try
        {
            await _apiService.PutAsync<ClientDto>($"clients/{id}", client);
            return true;
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error updating client {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        try
        {
            return await _apiService.DeleteAsync($"clients/{id}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error deleting client {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<VehicleDto>> GetClientVehiclesAsync(int clientId)
    {
        try
        {
            return await _apiService.GetAsync<IEnumerable<VehicleDto>>($"clients/{clientId}/vehicles");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Error getting vehicles for client {clientId}: {ex.Message}");
            return Enumerable.Empty<VehicleDto>();
        }
    }
}