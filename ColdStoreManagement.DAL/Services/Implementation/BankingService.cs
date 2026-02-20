using ColdStoreManagement.BLL.Models.Bank;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class BankingService(SQLHelperCore sql) : BaseService(sql), IBankingService
    {
        public async Task<List<BankModel>> GetBanks()
        {
            return await _sql.ExecuteReaderAsync<BankModel>(
                @"SELECT 
                    id     AS Bid,
                    Name   AS Bname,
                    Accno,
                    Branch,
                    ifsc   AS Ifsc,
                    accname AS Accname
                  FROM dbo.Bank_info",
                CommandType.Text
            );
        }
        public async Task<bool> AddBank(BankModel companyModel)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"INSERT INTO dbo.Bank_info
                  (Id, Name, branch, bankname, ifsc, accname, Accno)
                  VALUES (
                      (SELECT ISNULL(MAX(id)+1,1) FROM Bank_info),
                      @Name, @Branch, @BankName, @Ifsc, @Accname, @Accno
                  )",
                new SqlParameter("@Name", companyModel.Bname),
                new SqlParameter("@Branch", companyModel.Branch),
                new SqlParameter("@BankName", companyModel.Accname),
                new SqlParameter("@Ifsc", companyModel.Ifsc),
                new SqlParameter("@Accname", companyModel.Accname),
                new SqlParameter("@Accno", companyModel.Accno)
            );

            return true;
        }
        public async Task<bool> UpdateBank(int id, BankModel companyModel)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"UPDATE dbo.bank_info 
                  SET Name=@Bname,
                      Branch=@Branch,
                      BankName=@BankName,
                      Ifsc=@Ifsc,
                      Accname=@Accname,
                      Accno=@Accno
                  WHERE id=@Id",
                new SqlParameter("@Id", id),
                new SqlParameter("@Bname", companyModel.Bname),
                new SqlParameter("@Branch", companyModel.Branch),
                new SqlParameter("@BankName", companyModel.Accname),
                new SqlParameter("@Ifsc", companyModel.Ifsc),
                new SqlParameter("@Accname", companyModel.Accname),
                new SqlParameter("@Accno", companyModel.Accno)
            );

            return true;
        }
        public async Task<bool> DeleteBank(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE FROM dbo.Bank_info WHERE Id=@Id",
                new SqlParameter("@Id", id)
            );

            return true;
        }

        public async Task<CompanyModel?> AddTransaction(AddTransactionRequest request, int createdBy)
        {
            if (request == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddTransaction",
                new SqlParameter("@CreditGrp", request.CreditGroupId),
                new SqlParameter("@DebitGrp", request.DebitGroupId),
                new SqlParameter("@TransactionType", request.TransactionType),
                new SqlParameter("@PaymentType", request.PaymentType),
                new SqlParameter("@Dated", request.TransactionDate),
                new SqlParameter("@Remarks", request.Remarks),
                new SqlParameter("@Chno", request.ChequeNo),
                new SqlParameter("@Refno", request.ReferenceNo),
                new SqlParameter("@Amount", request.Amount),
                new SqlParameter("@Createdby", createdBy)
               // new SqlParameter("@Createdby", 1)
                
            );

            var result = new CompanyModel();
            await FillValidationAsync(result);

            return result;
        }
        public async Task<CompanyModel?> UpdateTransaction(UpdateTransactionRequest request, int updatedBy)
        {
            if (request == null)
                return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "UpdateTransaction",
                new SqlParameter("@TransactionId", request.TransactionId),
                new SqlParameter("@CreditGrp", request.CreditGroupId),
                new SqlParameter("@DebitGrp", request.DebitGroupId),
                new SqlParameter("@TransactionType", request.TransactionType),
                new SqlParameter("@PaymentType", request.PaymentType),
                new SqlParameter("@Dated", request.TransactionDate),
                new SqlParameter("@Remarks", request.Remarks),
                new SqlParameter("@Chno", request.ChequeNo),
                new SqlParameter("@Refno", request.ReferenceNo),
                new SqlParameter("@Amount", request.Amount),
                new SqlParameter("@Updatedby", updatedBy)
            );

            var result = new CompanyModel();
            await FillValidationAsync(result);

            return result;
        }


    }
}
