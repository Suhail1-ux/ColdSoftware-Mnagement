using ColdStoreManagement.BLL.Models.TransactionsIn;
using ColdStoreManagement.DAL.Services.Interface;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ColdStoreManagement.DAL.Helper;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class StockAggregateService : IStockAggregateService
    {
        private readonly SQLHelperCore _sqlHelper;

        public StockAggregateService(SQLHelperCore sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<List<TransactionsInModel>> GetStockAggregateDataAsync()
        {
            const string query = @"SELECT p.partyid as Growerid, p.partytypeid + '-' + p.partyname AS GrowerGroupName,  
                                   CAST(sum(companycrate) AS DECIMAL(18,2)) as TotalPrecompanyQty,    
                                   CAST(sum(owncrate) AS DECIMAL(18,2)) as TotalPreownQty,    
                                   CAST(sum(qty) AS DECIMAL(18,2)) as TotalPreQty,    
                                   CAST(sum(VerifiedCompanyCrates) AS DECIMAL(18,2)) as TotalIncompanyQty,    
                                   CAST(sum(VerifiedOwnCrates) AS DECIMAL(18,2)) as TotalInownQty,    
                                   CAST(sum(VerifiedQty) AS DECIMAL(18,2)) as TotalInQty,    
                                   CAST(sum(VerifiedWoodenPetty) AS DECIMAL(18,2)) as TotalInpettyQty  
                                   FROM gateintrans ci 
                                   LEFT JOIN party p ON ci.partyid = p.partyid 
                                   WHERE ci.flagdeleted=0 and ci.dockpost=1 
                                   GROUP BY p.partyid, p.partytypeid + '-' + p.partyname 
                                   ORDER BY partyid";

            return await _sqlHelper.ExecuteReaderAsync<TransactionsInModel>(query, CommandType.Text);
        }

        public async Task<List<TransactionsInModel>> GetStockChamberDataAsync(int growerId)
        {
            const string query = @"SELECT p.chambername as ChamberName,
                                   CAST(sum(companycrate) AS DECIMAL(18,2)) as TotalPrecompanyQty,
                                   CAST(sum(owncrate) AS DECIMAL(18,2)) as TotalPreownQty,
                                   CAST(sum(ci.qty) AS DECIMAL(18,2)) as TotalPreQty,
                                   CAST(sum(VerifiedCompanyCrates) AS DECIMAL(18,2)) as TotalIncompanyQty,
                                   CAST(sum(VerifiedOwnCrates) AS DECIMAL(18,2)) as TotalInownQty,
                                   CAST(sum(VerifiedQty) AS DECIMAL(18,2)) as TotalInQty,
                                   CAST(sum(VerifiedWoodenPetty) AS DECIMAL(18,2)) as TotalInpettyQty  
                                   FROM gateintrans ci 
                                   LEFT JOIN chamber p ON ci.LocationChamberId = p.chamberid 
                                   WHERE ci.flagdeleted=0 and ci.dockpost=1 and ci.PartyId=@Growerid 
                                   GROUP BY p.chambername, p.chamberid 
                                   ORDER BY p.chamberid";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Growerid", growerId)
            };

            return await _sqlHelper.ExecuteReaderAsync<TransactionsInModel>(query, CommandType.Text, parameters.ToArray());
        }
    }
}
