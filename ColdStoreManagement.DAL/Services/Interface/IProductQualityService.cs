using ColdStoreManagement.BLL.Models.Product;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IProductQualityService
    {
        Task<bool> DoesQualityExistAsync(string name);
        Task<List<ProductQualityModel>> GetAllQualitiesAsync();
        Task<ProductQualityModel?> GetByIdAsync(int id);
        Task<bool> AddQualityAsync(ProductQualityModel model);
        Task<bool> UpdateQualityAsync(int id, ProductQualityModel model);
        Task<bool> DeleteQualityAsync(int id);
    }
}
