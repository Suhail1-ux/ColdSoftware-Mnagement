using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ColdStoreManagement.DAL.Services.Implementation.TransactionsOut
{
    public class DemandOrderService : IDemandOrderService
    {
        private readonly IConfiguration _configuration;

        public DemandOrderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> UpdateLotOrderQuantity(CompanyModel EditModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "update LotOutTranstemp set OrderQty=@oqty where lotno=@Lotno and Tempid=@Tempid";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text,
                };
                cmd.Parameters.AddWithValue("@oqty", EditModel.OrderQty);
                cmd.Parameters.AddWithValue("@Lotno", EditModel.Lotno);
                cmd.Parameters.AddWithValue("@Tempid", EditModel.TempOrderId);

                con.Open();
                await cmd.ExecuteNonQueryAsync();
                con.Close();
                cmd.Dispose();
            }
            return true;
        }

        public async Task<List<CompanyModel>> GenerateTempLotReport(CompanyModel EditModel, int unit, int Tempid)
        {
            List<CompanyModel> GetDailyPreinwardTemp = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT 
    g.LOTNO,g.lotirn,
    g.VerifiedQty AS GateInQty,c.partytypeid  +'-'+ c.partyname as PartyName,  CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
    g.orderqty AS OrderQty, g.orderqty AS OrderQty,  g.verifiedQty-g.OrderQty as NetQty,
    (g.VerifiedQty - COALESCE(SUM(o.QTY), 0)) AS BalanceQty,g.verifiedQty ,prt.name as itemname, pt.name as package,g.khataname,pq.name as quality,
	lf.FinalLocationText as tlocation,cr.Chambername as chamber,cm.ChallanName,st.sname
FROM LotOutTranstemp g

LEFT JOIN LOTOUTTRANS o ON g.LOTNO = o.LOTNO
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id  
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN chamber cr ON g.locationchamberid =cr.chamberid

LEFT JOIN locationFinal lf ON g.lotno =lf.lotno
where g.tempid=@Tempid and g.unitid=@unit
GROUP BY g.orderqty,g.LOTNO, g.VerifiedQty,c.partytypeid  +'-'+ c.partyname ,  CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname,prt.name ,pt.name,g.khataname,pq.name,cm.ChallanName,st.sname,
FinalLocationText,cr.Chambername,g.lotirn

ORDER BY g.LOTNO";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Tempid", Tempid);
                cmd.Parameters.AddWithValue("@unit", unit);
                con.Open();

                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        Lotno = Convert.ToInt32(rdr["LOTNO"]),
                        LotIrn = rdr["lotirn"].ToString(),
                        GrowerGroupName = rdr["PartyName"].ToString(),
                        GrowerName = rdr["GrowerName"].ToString(),
                        OrderQty = Convert.ToInt32(rdr["OrderQty"]),
                        GrowerCombine = rdr["PartyName"].ToString() + " " + rdr["GrowerName"].ToString(),
                        Netqty = Convert.ToInt32(rdr["NetQty"]),
                        VerfiedQty = Convert.ToInt32(rdr["VerifiedQty"]),
                        Alotbal = Convert.ToInt32(rdr["BalanceQty"]),
                        ItemName = rdr["itemname"].ToString(),
                        Prodetails = rdr["package"].ToString(),
                        PreInwardKhata = rdr["khataname"].ToString(),
                        VarietyId = rdr["quality"].ToString(),
                        ChamberName = rdr["Chamber"].ToString(),
                    };
                    GetDailyPreinwardTemp.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return GetDailyPreinwardTemp;
        }

        public async Task<List<CompanyModel>> GenerateTempLotRawReportEdit(CompanyModel EditModel, int unit, int Tempid)
        {
            List<CompanyModel> GetDailyPreinwardTemp = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd3 = new SqlCommand("ProcessEditDemandRaw", con))
                {
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@Tempid", Tempid);
                    await cmd3.ExecuteNonQueryAsync();
                }

                const string query = @"SELECT g.Tempid,
    g.LOTNO,g.lotirn,
    g.VerifiedQty AS GateInQty,c.partytypeid  +'-'+ c.partyname as PartyName,  CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname AS GrowerName,
    g.orderqty AS OrderQty, g.orderqty AS OrderQty,  g.verifiedQty-g.OrderQty as NetQty,
    (g.VerifiedQty - COALESCE(SUM(o.OrderQty), 0)) AS BalanceQty,g.verifiedQty ,prt.name as itemname, pt.name as package,g.khataname,pq.name as quality,g.locationchamberid,
	isnull(FloorName+space(1)+lf.MatrixName+lf.RowName+space(1)+lf.ColumnName ,'NA') as tlocation,g.locationchamberid as chamber,cm.ChallanName,st.sname
FROM LotOutTranstemp g

LEFT JOIN LOTOUTTRANS o ON g.LOTNO = o.LOTNO
LEFT JOIN PARTY C ON G.PARTYID = C.PARTYID
LEFT JOIN PARTYsub ps ON G.GrowerId = PS.PARTYID
LEFT JOIN prodtype prt ON g.itemid = prt.id
LEFT JOIN PTYPE pt ON g.packageid = pt.id
LEFT JOIN prodqaul pq ON g.varietyid = pq.id
LEFT JOIN challanmaster cm ON g.challanid = cm.id  
LEFT JOIN servicetypes st ON g.schemeid = st.id
LEFT JOIN locationFinal lf ON g.lotno =lf.lotno
where g.tempid in(select tempid from LotOutTranstemp where outid=@Tempid) and g.unitid=@unit and g.flagdeleted=0
GROUP BY  g.Tempid,g.orderqty,g.LOTNO, g.VerifiedQty,c.partytypeid  +'-'+ c.partyname ,  CONVERT(varchar(max), ps.partyid) + '-' + ps.partyname,prt.name ,pt.name,g.khataname,pq.name,cm.ChallanName,st.sname,
FloorName+space(1)+lf.MatrixName+lf.RowName+space(1)+lf.ColumnName,g.locationchamberid,g.lotirn

ORDER BY g.LOTNO";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Tempid", Tempid);
                cmd.Parameters.AddWithValue("@unit", unit);

                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        Lotno = Convert.ToInt32(rdr["LOTNO"]),
                        LotIrn = rdr["lotirn"].ToString(),
                        GrowerGroupName = rdr["PartyName"].ToString(),
                        OrderQty = Convert.ToInt32(rdr["OrderQty"]),
                        GrowerCombine = rdr["PartyName"].ToString() + " " + rdr["GrowerName"].ToString(),
                        Netqty = Convert.ToInt32(rdr["NetQty"]),
                        VerfiedQty = Convert.ToInt32(rdr["VerifiedQty"]),
                        Alotbal = Convert.ToInt32(rdr["BalanceQty"]) + Convert.ToInt32(rdr["OrderQty"]),
                        ItemName = rdr["itemname"].ToString(),
                        Prodetails = rdr["package"].ToString(),
                        PreInwardKhata = rdr["khataname"].ToString(),
                        VarietyId = rdr["quality"].ToString(),
                        ChamberId = Convert.ToInt32(rdr["locationchamberid"]),
                        Templocation = rdr["tlocation"].ToString(),
                        ChallanName = rdr["ChallanName"].ToString(),
                        ServiceId = rdr["sname"].ToString(),
                    };
                    GetDailyPreinwardTemp.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return GetDailyPreinwardTemp;
        }
    }
}
