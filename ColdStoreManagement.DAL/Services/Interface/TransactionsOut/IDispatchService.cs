using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IDispatchService
    {
        Task<CompanyModel?> GetDispatchPriv(string Ugroup);
        Task<List<CompanyModel>> GenerateDispatchReport(string packgrower, string Tlocation, int unit, CompanyModel EditModel);
        Task<List<CompanyModel>> GenerateDispatchReportComplete(string packgrower, string Tlocation, int unit, CompanyModel EditModel);
        Task<CompanyModel?> ValidatePelletDispatch(CompanyModel companyModel, int unit);
        Task<CompanyModel?> AddFinalDispatch(CompanyModel EditModel, int unit);
        Task<CompanyModel?> Deletedispatch(int id, CompanyModel companyModel, int unit);
        Task<List<CompanyModel>> GenerateDispatchReportpend(int unit, CompanyModel EditModel);
    }
}
