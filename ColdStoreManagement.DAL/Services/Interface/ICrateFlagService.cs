using ColdStoreManagement.BLL.Models.Crate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICrateFlagService
    {
        Task<List<CrateFlags>> GetAllCrateFlagsAsync();
        Task<bool> DoesCrateFlagExistAsync(string name);
        Task<bool> AddCrateFlagAsync(CrateFlags model);
        Task<bool> UpdateCrateFlagAsync(CrateFlags model);
        Task<bool> DeleteCrateFlagAsync(int id);
    }
}
