using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IPackingOrderService
    {
        Task<CompanyModel?> GetPackingOrderPriv(string Ugroup);
        Task<List<CompanyModel>> GetAllOrderby(int UnitId);
        Task<List<CompanyModel>> generateCompletepackingorderOpen(CompanyModel EditModel, int unit);
        Task<bool> UpdatePackingorderStatus(int id, CompanyModel companyModel);
        Task<CompanyModel?> DeletePackingOrder(int selectedGrowerId, CompanyModel companyModel);
        Task<CompanyModel?> AssignpackingId(string uname, int packingorder, CompanyModel EditModel);
        Task<List<CompanyModel>> GetAllPackingOrders(int UnitId);
        Task<List<CompanyModel>> GetallDrafts(int UnitId);
    }
}
