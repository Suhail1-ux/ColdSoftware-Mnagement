using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IStoreOutService
    {
        Task<List<CompanyModel>> GetStoreOutStatus(string stat, int UnitId, string demandirn, string avuser);
        Task<bool> UpdateDraftQuantity(CompanyModel EditModel);
        Task<bool> ForceUpload(int id, string Frems);
        Task<CompanyModel?> ValidateStoreOutTransQty(CompanyModel companyModel);
        Task<CompanyModel?> AddStoreOut(CompanyModel companyModel);
    }
}
