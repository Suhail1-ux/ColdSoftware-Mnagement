using ColdStoreManagement.BLL.Models;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IUnitService
    {
        Task<List<UnitMasterModel>> GetAllAsync();
        Task<UnitMasterModel?> GetByIdAsync(int id);
        Task<UnitMasterModel?> GetByNameAsync(string unitName);
        Task<bool> AddAsync(UnitMasterModel model);
        Task<bool> UpdateAsync(int id, UnitMasterModel model);
        Task<bool> DeleteAsync(int id);
    }
}
