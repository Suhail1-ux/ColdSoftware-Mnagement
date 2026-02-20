using ColdStoreManagement.BLL.Models;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IServiceTypeService
    {
        Task<bool> DoesServiceExistAsync(string serviceName);
        Task<List<ServiceTypesModel>> GetServices();
        Task<List<ServiceTypesModel>> GetServicesFromAgreement(string selectedPurchase);
        Task<List<ServiceTypesModel>> GetAllServices();
        Task<ServiceTypesModel?> GetServiceById(int id);

        Task<bool> AddService(ServiceTypesModel model);
        Task<bool> UpdateService(int id, ServiceTypesModel model);
        Task<bool> DeleteService(int id); 
    }
}
