using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ColdStoreManagement.DAL.Services.Implementation.TransactionsOut
{
    public class PackingOrderService : IPackingOrderService
    {
        private readonly IConfiguration _configuration;

        public PackingOrderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CompanyModel?> GetPackingOrderPriv(string Ugroup)
        {
            CompanyModel? companyModel = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string query = "select Addval,Editval,ViewVal,DelVal,Approval from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Ugroup", Ugroup);
                    cmd.Parameters.AddWithValue("@pname", "Packing Order");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {
                                PackingOrderAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                PackingOrderEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                PackingOrderView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                PackingOrderDel = reader.GetBoolean(reader.GetOrdinal("DelVal")),
                                PackingOrderApp = reader.GetBoolean(reader.GetOrdinal("Approval")),
                            };
                        }
                    }
                }
            }
            return companyModel;
        }

        public async Task<List<CompanyModel>> GetAllOrderby(int UnitId)
        {
            List<CompanyModel> GetbyOrderList = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "SELECT distinct OrderByName FROM LOTOUTTRANS WHERE flagdeleted=0 and unitid=@Unitid";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Unitid", UnitId);

                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        OrderBy = rdr["OrderByName"].ToString(),
                    };
                    GetbyOrderList.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return GetbyOrderList;
        }

        public async Task<List<CompanyModel>> generateCompletepackingorderOpen(CompanyModel EditModel, int unit)
        {
            List<CompanyModel> GetDailyPreinward = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd3 = new SqlCommand("generatepackingorders", con))
                {
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@d1", EditModel.PackingOrderDatefrom);
                    cmd3.Parameters.AddWithValue("@d2", EditModel.PackingOrderDateto);
                    cmd3.Parameters.AddWithValue("@PackingId", EditModel.PackingOrderIrn);
                    cmd3.Parameters.AddWithValue("@DraftIrn", EditModel.DraftIrn);
                    cmd3.Parameters.AddWithValue("@Draftstat", EditModel.PackingOrderStatus);
                    cmd3.Parameters.AddWithValue("@unit", unit);

                    await cmd3.ExecuteNonQueryAsync();
                }
                con.Close();

                const string query = @"WITH DistinctDrafts AS (
    SELECT packingorderid, packingorderno, draftid, draftirn, Vehid, qty, PackingOrderDate, packingorderstat,
           CONVERT(varchar(40), packingorderdate, 104) AS DrafDate, partyid, growerid, challanid
    FROM PackingOrderItemsreport WHERE flagdeleted=0
),
DistinctDemands AS (
    SELECT packingorderno, STRING_AGG(draftirn, ', ') AS DraftIRNs, COUNT(DISTINCT draftirn) AS DraftCount
    FROM (SELECT DISTINCT packingorderno, draftirn FROM DistinctDrafts WHERE draftirn IS NOT NULL) d
    GROUP BY packingorderno
),
DistinctGrowers AS (
    SELECT packingorderno, STRING_AGG(CONVERT(VARCHAR(MAX), partyid) + '-' + partyname, CHAR(13) + CHAR(10)) AS GrowerName
    FROM (SELECT DISTINCT dd.packingorderno, ps.partyid, ps.partyname FROM DistinctDrafts dd LEFT JOIN partysub ps ON dd.growerid = ps.partyid WHERE ps.partyid IS NOT NULL) g
    GROUP BY packingorderno
),
OrderAggregates AS (
    SELECT packingorderno, SUM(qty) AS TotalGradingQty FROM DistinctDrafts GROUP BY packingorderno
)
SELECT dd.packingorderid, dd.packingorderno, dem.DraftIRNs, dem.DraftCount, dd.packingorderstat, dd.PackingOrderDate,
       oa.TotalGradingQty, p.partytypeid + '-' + p.partyname AS PartyInfo, ps.partytypeid + '-' + ps.partyname AS GrowerInfo,
       cm.ChallanName, vh.vehno
FROM (SELECT DISTINCT packingorderid, packingorderno, packingorderstat, PackingOrderDate, partyid, vehid, growerid, ChallanId FROM DistinctDrafts) dd
LEFT JOIN DistinctDemands dem ON dd.packingorderno = dem.packingorderno
LEFT JOIN DistinctGrowers gr ON dd.packingorderno = gr.packingorderno
LEFT JOIN OrderAggregates oa ON dd.packingorderno = oa.packingorderno
LEFT JOIN party p ON dd.partyid = p.partyid
LEFT JOIN partysub ps ON dd.growerid = ps.partyid
LEFT JOIN ChallanMaster cm ON dd.challanid = cm.Id
LEFT JOIN vehinfo vh ON dd.vehid = vh.vid
ORDER BY dd.packingorderno desc";

                SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text };
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    GetDailyPreinward.Add(new CompanyModel
                    {
                        Packingorderid = Convert.ToInt32(rdr["packingorderid"]),
                        PackingOrderIrn = rdr["packingorderno"].ToString(),
                        DraftIrn = rdr["DraftIRNs"].ToString(),
                        PackingOrderDate = Convert.ToDateTime(rdr["PackingOrderDate"]),
                        PackingOrderStatus = rdr["packingorderstat"].ToString(),
                        selectedPackingQty = Convert.ToInt32(rdr["TotalGradingQty"]),
                        GrowerGroupName = rdr["PartyInfo"].ToString(),
                        GrowerName = rdr["GrowerInfo"].ToString(),
                        ChallanName = rdr["ChallanName"].ToString(),
                        Vehno = rdr["vehno"].ToString(),
                    });
                }
                con.Close();
            }
            return GetDailyPreinward;
        }

        public async Task<bool> UpdatePackingorderStatus(int id, CompanyModel companyModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                using (SqlCommand cmd = new SqlCommand("UpdatePackingorderStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Growerid", id);
                    con.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return true;
        }

        public async Task<CompanyModel?> DeletePackingOrder(int selectedGrowerId, CompanyModel companyModel)
        {
            if (companyModel == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("DeletePackingOrder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mid", selectedGrowerId);
                    cmd.Parameters.AddWithValue("@User", companyModel.GlobalUserName);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (rdr.Read())
                        {
                            companyModel.RetMessage = rdr["remarks"]?.ToString();
                            companyModel.RetFlag = rdr["flag"]?.ToString();
                        }
                    }
                }
            }
            return companyModel;
        }

        public async Task<CompanyModel?> AssignpackingId(string uname, int packingorder, CompanyModel EditModel)
        {
            if (EditModel == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("assignpackingorder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uname", uname);
                    cmd.Parameters.AddWithValue("@Pod", packingorder);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con))
                {
                    using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                    {
                        if (rdr.Read())
                        {
                            EditModel.RetMessage = rdr["remarks"]?.ToString();
                            EditModel.RetFlag = rdr["flag"]?.ToString();
                        }
                    }
                }
            }
            return EditModel;
        }

        public async Task<List<CompanyModel>> GetAllPackingOrders(int UnitId)
        {
            List<CompanyModel> PackingOrderList = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "SELECT distinct packingorderno FROM packingorderitems WHERE flagdeleted=0 and unitid=@Unitid";
                SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@Unitid", UnitId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    PackingOrderList.Add(new CompanyModel { PackingOrderIrn = rdr["packingorderno"].ToString() });
                }
                con.Close();
            }
            return PackingOrderList;
        }

        public async Task<List<CompanyModel>> GetallDrafts(int UnitId)
        {
            List<CompanyModel> DraftList = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "SELECT distinct Draftirn FROM DraftOutTrans WHERE flagdeleted=0 and unitid=@Unitid";
                SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@Unitid", UnitId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    DraftList.Add(new CompanyModel { DraftIrn = rdr["Draftirn"].ToString() });
                }
                con.Close();
            }
            return DraftList;
        }
    }
}
