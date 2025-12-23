using Autoservice156MAUI.Models.DTO;

namespace Autoservice156MAUI.Services.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
    Task<VehicleDto> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicle);
    Task<bool> UpdateVehicleAsync(int id, VehicleDto vehicle);
    Task<bool> DeleteVehicleAsync(int id);
    Task<IEnumerable<VehicleDto>> GetVehiclesByClientIdAsync(int clientId);
}