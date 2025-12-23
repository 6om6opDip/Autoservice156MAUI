using Autoservice156MAUI.Models.DTO;

namespace Autoservice156MAUI.Services.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<ClientDto> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(ClientDto client);
    Task<bool> UpdateClientAsync(int id, ClientDto client);
    Task<bool> DeleteClientAsync(int id);
    Task<IEnumerable<VehicleDto>> GetClientVehiclesAsync(int clientId);
}