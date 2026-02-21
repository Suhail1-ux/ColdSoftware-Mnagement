using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface IDemandOrderService
    {
        Task<bool> UpdateLotOrderQuantity(CompanyModel EditModel);
        Task<List<CompanyModel>> GenerateTempLotReport(CompanyModel EditModel, int unit, int Tempid);
        Task<List<CompanyModel>> GenerateTempLotRawReportEdit(CompanyModel EditModel, int unit, int Tempid);
    }
}
