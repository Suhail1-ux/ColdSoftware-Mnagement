using ColdStoreManagement.BLL.Models.Reports;
using ColdStoreManagement.BLL.Models.TransactionsIn;
using ColdStoreManagement.DAL.Services.Interface;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ColdStoreManagement.DAL.Helper;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class PreinwardReportService : IPreinwardReportService
    {
        private readonly SQLHelperCore _sqlHelper;

        public PreinwardReportService(SQLHelperCore sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<List<TransactionsInModel>> GetReportDataAsync(ReportRequestModel request)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@d1", request.DateFrom),
                new SqlParameter("@d2", request.DateTo),
                new SqlParameter("@Partyid", request.PartyId),
                new SqlParameter("@Groowerid", request.GrowerId),
                new SqlParameter("@vehname", request.VehicleNo ?? (object)DBNull.Value),
                new SqlParameter("@itemname", request.ProductName ?? (object)DBNull.Value),
                new SqlParameter("@uom", request.Uom ?? (object)DBNull.Value),
                new SqlParameter("@stat", request.Status ?? (object)DBNull.Value),
                new SqlParameter("@vehid", 0),
                new SqlParameter("@itemid", 0),
                new SqlParameter("@uomid", 0),
                new SqlParameter("@SearchText", request.CommonSearch ?? (object)DBNull.Value)
            };

            await _sqlHelper.ExecuteNonQueryAsync(CommandType.StoredProcedure, "generatePreinwardReport", parameters.ToArray());

            const string query = @"SELECT ci.cid as PreInwardId, ci.Trno as PreInIrn, ci.Dated as PreinwardDate, 
                                   p.partytypeid + '-' + p.partyname AS GrowerGroupName, 
                                   convert(varchar(max),ps.partyid) + '-' + ps.partyname AS GrowerName,
                                   convert(varchar(max),cm.id) + '-' + cm.ChallanName as ChallanName,
                                   rtrim(vi.vehno) + ' (' + rtrim(vi.drivername) + ') (' + rtrim(vi.contactno) + ')' as Vehno,
                                   ci.CrateMark as CrateMarka, ci.qty as PreInwardQty, ci.trflag as RetFlag, ci.Remarks as PreInwardRemarks
                                   FROM CrateIssuereports ci
                                   LEFT JOIN party p ON ci.partyid = p.partyid
                                   LEFT JOIN partysub ps ON ci.groowerid = ps.partyid
                                   LEFT JOIN challanmaster cm ON ci.challanid = cm.id         
                                   LEFT JOIN vehinfo vi ON ci.vehid = vi.vid
                                   WHERE ci.flagdeleted=0 order by ci.cid";

            return await _sqlHelper.ExecuteReaderAsync<TransactionsInModel>(query, CommandType.Text);
        }
    }
}
