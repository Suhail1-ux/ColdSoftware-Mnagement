using ColdStoreManagement.BLL.Models.TransactionsIn;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IStockAggregateService
    {
        Task<List<TransactionsInModel>> GetStockAggregateDataAsync();
        Task<List<TransactionsInModel>> GetStockChamberDataAsync(int growerId);
    }
}
