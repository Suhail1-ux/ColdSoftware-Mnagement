using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.Product;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IPackageService
    {
        Task<List<PackageModel>> GetAllPackagesAsync();
        Task<PackageModel?> GetByIdAsync(int id);
        Task<bool> AddPackageAsync(PackageModel model);
        Task<bool> UpdatePackageAsync(int id, PackageModel model);
        Task<bool> DeletePackageAsync(int id);
    }
}
