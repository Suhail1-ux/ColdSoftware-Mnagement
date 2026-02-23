using ColdStoreManagement.BLL.Models.Reports;
using ColdStoreManagement.BLL.Models.TransactionsIn;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IInwardReportService
    {
        Task<List<TransactionsInModel>> GetReportDataAsync(ReportRequestModel request);
    }
}
