using ColdStoreManagement.BLL.Models.Reports;
using ColdStoreManagement.BLL.Models.TransactionsIn;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IQualityReportService
    {
        Task<List<TransactionsInModel>> GetReportDataAsync(ReportRequestModel request);
    }
}
