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
    public class StoreOutService : IStoreOutService
    {
        private readonly IConfiguration _configuration;

        public StoreOutService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<CompanyModel>> GetStoreOutStatus(string stat, int UnitId, string demandirn, string avuser)
        {
            List<CompanyModel> GetDailyPreinward = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT 
    l.isAssigned AS Assign,
    ps.partytypeid + '-' + ps.partyname AS PartyName,
    l.demandirn,
    l.outid,
    MAX(l.otype) AS otype,
    SUM(COALESCE(l.orderqty, 0)) AS TotalLotOrderQty,
    ISNULL((
        SELECT SUM(COALESCE(s.orderqty, 0)) 
        FROM StoreOutTrans s 
        WHERE s.outid = l.outid 
    ), 0) AS TotalStoreOrderQty,
    ISNULL((
        SELECT SUM(COALESCE(df.orderqty, 0)) 
        FROM DraftOutTrans df 
        WHERE df.outid = l.outid 
    ), 0) AS TotalDraftedQty,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM StoreOutTrans s 
            WHERE s.outid = l.outid 
            AND s.PreLotPrefix = 'FORCE'
        ) THEN 'Completed'
        WHEN SUM(COALESCE(l.orderqty, 0)) = ISNULL((
            SELECT SUM(COALESCE(s.orderqty, 0)) 
            FROM StoreOutTrans s 
            WHERE s.outid = l.outid
        ), 0) THEN 'Completed'
        ELSE 'Pending'
    END AS Status
FROM LotOutTrans l
LEFT OUTER JOIN party ps ON ps.partyid = l.PartyId 
WHERE 
    (
        (@status = 'Completed' AND (
            l.outid IN (
                SELECT s.outid 
                FROM StoreOutTrans s 
                GROUP BY s.outid
                HAVING SUM(COALESCE(s.orderqty, 0)) = (
                    SELECT SUM(COALESCE(l2.orderqty, 0))
                    FROM LotOutTrans l2
                    WHERE l2.outid = s.outid
                )
            )
            OR EXISTS (
                SELECT 1 
                FROM StoreOutTrans s 
                WHERE s.outid = l.outid 
                AND s.PreLotPrefix = 'FORCE'
            )
        ))
        OR 
        (@status = 'Pending' AND (
            l.outid NOT IN (
                SELECT s.outid 
                FROM StoreOutTrans s 
                GROUP BY s.outid
                HAVING SUM(COALESCE(s.orderqty, 0)) = (
                    SELECT SUM(COALESCE(l2.orderqty, 0))
                    FROM LotOutTrans l2
                    WHERE l2.outid = s.outid
                )
            )
            AND NOT EXISTS (
                SELECT 1 
                FROM StoreOutTrans s 
                WHERE s.outid = l.outid 
                AND s.PreLotPrefix = 'FORCE'
            )
        ))
        OR (@status IS NULL OR @status = '')
    )
    AND l.UnitId = @unit 
    AND l.DemandStatus = 'Approved'
    AND (@demandirn IS NULL OR @demandirn = '' OR l.demandirn = @demandirn)
GROUP BY 
    l.outid,
    l.isAssigned,
    l.demandirn,
    ps.partytypeid + '-' + ps.partyname
ORDER BY l.outid desc";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@status", stat);
                cmd.Parameters.AddWithValue("@unit", UnitId);
                cmd.Parameters.AddWithValue("@demandirn", demandirn);
                cmd.Parameters.AddWithValue("@username", avuser);

                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        IsuserAssigned = rdr.GetBoolean(rdr.GetOrdinal("Assign")),
                        DemandIrn = rdr["demandirn"].ToString(),
                        GrowerGroupName = rdr["PartyName"].ToString(),
                        OrderType = rdr["otype"].ToString(),
                        TotalOrderQty = Convert.ToInt32(rdr["TotalLotOrderQty"]),
                        TotalStoreOut = Convert.ToInt32(rdr["TotalStoreOrderQty"]),
                        DraftedQty = Convert.ToInt32(rdr["TotalDraftedQty"]),
                        DemandNo = Convert.ToInt32(rdr["outid"]),
                        StoreStat = rdr["Status"].ToString(),
                    };
                    GetDailyPreinward.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return GetDailyPreinward;
        }

        public async Task<bool> UpdateDraftQuantity(CompanyModel EditModel)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "update StoreOutTransrep set count=1,OrderQty=@oqty where lotno=@Lotno and TempDraftId=@Tempid";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text,
                };
                cmd.Parameters.AddWithValue("@oqty", EditModel.GradingQty);
                cmd.Parameters.AddWithValue("@Lotno", EditModel.Lotno);
                cmd.Parameters.AddWithValue("@Tempid", EditModel.TempDraftId);

                con.Open();
                await cmd.ExecuteNonQueryAsync();
                con.Close();
                cmd.Dispose();
            }
            return true;
        }

        public async Task<bool> ForceUpload(int id, string Frems)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = "update StoreOutTrans set PreLotPrefix=@fix,remarks=@Pkname where outid=@Id";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text,
                };
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Pkname", Frems);
                cmd.Parameters.AddWithValue("@fix", "FORCE");

                con.Open();
                await cmd.ExecuteNonQueryAsync();
                con.Close();
                cmd.Dispose();
            }
            return true;
        }

        public async Task<CompanyModel?> ValidateStoreOutTransQty(CompanyModel companyModel)
        {
            if (companyModel == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("ValidateStoreOutTransQty", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OutID", companyModel.DemandNo);
                    cmd.Parameters.AddWithValue("@Lotno", companyModel.Lotno);
                    cmd.Parameters.AddWithValue("@NewQty", companyModel.TotalStoreOut);
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
                con.Close();
            }
            return companyModel;
        }

        public async Task<CompanyModel?> AddStoreOut(CompanyModel companyModel)
        {
            if (companyModel == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("AddStoreOut", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OutID", companyModel.DemandNo);
                    cmd.Parameters.AddWithValue("@Lotno", companyModel.Lotno);
                    cmd.Parameters.AddWithValue("@NewQty", companyModel.TotalStoreOut);
                    cmd.Parameters.AddWithValue("@Createdby", companyModel.GlobalUserName);
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
                con.Close();
            }
            return companyModel;
        }
    }
}
