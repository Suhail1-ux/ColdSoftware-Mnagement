using ColdStoreManagement.BLL.Models.Crate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICrateTypeService
    {
        Task<List<CrateType>> GetAllCrateTypesAsync();
        Task<bool> DoesCrateTypeExistAsync(string name);
        Task<bool> AddCrateTypeAsync(CrateType model);
        Task<bool> UpdateCrateTypeAsync(CrateType model);
        Task<bool> DeleteCrateTypeAsync(int id);
    }
}
