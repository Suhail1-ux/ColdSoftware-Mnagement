using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IPackingDraftService
    {
        Task<CompanyModel?> ValidateDraftQty(CompanyModel companyModel);
        Task<List<CompanyModel>> GetallDraftno(int Unit);
        Task<List<CompanyModel>> GetallDraftnobill(int Unit);
        Task<CompanyModel?> AddFinalDraft(CompanyModel EditModel);
        Task<bool> UpdateDraftQuantity(CompanyModel EditModel);
    }
}
