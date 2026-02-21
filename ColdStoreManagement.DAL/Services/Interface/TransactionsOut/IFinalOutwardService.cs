using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IFinalOutwardService
    {
        Task<CompanyModel?> FInaloutwardPriv(string Ugroup);
        Task<CompanyModel?> AddFinaloutward(CompanyModel EditModel, int unit);
        Task<CompanyModel?> UpdateFinaloutward(List<CompanyModel> checkedPellets, CompanyModel baseEditModel, int unit);
    }
}
