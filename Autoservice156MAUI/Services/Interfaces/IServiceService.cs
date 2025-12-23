using Autoservice156MAUI.Models.DTO;

namespace Autoservice156MAUI.Services.Interfaces;

public interface IServiceService
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    Task<ServiceDto> GetServiceByIdAsync(int id);
    Task<ServiceDto> CreateServiceAsync(ServiceDto service);
    Task<bool> UpdateServiceAsync(int id, ServiceDto service);
    Task<bool> DeleteServiceAsync(int id);
    Task<IEnumerable<ServiceDto>> GetActiveServicesAsync();
}