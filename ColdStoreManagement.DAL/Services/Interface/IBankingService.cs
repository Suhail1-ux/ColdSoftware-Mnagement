using ColdStoreManagement.BLL.Models.Bank;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public  interface IBankingService
    {
        // Bank
        Task<List<BankModel>> GetBanks();
        Task<bool> AddBank(BankModel companyModel);
        Task<bool> UpdateBank(int id, BankModel companyModel);
        Task<bool> DeleteBank(int id);


        // Transactions
        Task<CompanyModel?> AddTransaction(AddTransactionRequest request, int createdBy);
        Task<CompanyModel?> UpdateTransaction(UpdateTransactionRequest request, int updatedBy);
        //Task<List<BankTransactionModel>> GetBankTransactionsAsync();
    }
}
