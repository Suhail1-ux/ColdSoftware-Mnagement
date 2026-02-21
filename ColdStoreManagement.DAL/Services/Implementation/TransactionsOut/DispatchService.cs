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
    public class DispatchService : IDispatchService
    {
        private readonly IConfiguration _configuration;

        public DispatchService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CompanyModel?> GetDispatchPriv(string Ugroup)
        {
            CompanyModel? companyModel = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string query = "select Addval,Editval,ViewVal,DelVal from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Ugroup", Ugroup);
                    cmd.Parameters.AddWithValue("@pname", "Dispatch");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {
                                DispatchAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                DispatchEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                DispatchView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                DispatchDel = reader.GetBoolean(reader.GetOrdinal("DelVal")),
                            };
                        }
                    }
                }
            }
            return companyModel;
        }

        public async Task<List<CompanyModel>> GenerateDispatchReport(string packgrower, string Tlocation, int unit, CompanyModel EditModel)
        {
            List<CompanyModel> GetPackingStock = new List<CompanyModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT p.draftid, p.palletno, p.PackingOrderId, p.PackingOrderno, p.palletIrn, lm.name, ur.username,
                                     c.partytypeid + '-' + c.partyname as PartyName, p.Palletno, p.marka, ig.gradename, pq.name as variety,
                                     rc.recipename, p.qty AS PackingQty, COALESCE(d.TotalDispatchedQty, 0) AS TotalDispatchedQty,
                                     (p.Qty - COALESCE(d.TotalDispatchedQty, 0)) AS AvailableQty
                                     FROM packingorderitems p
                                     LEFT JOIN (SELECT PackingOrderId, palletno, unitid, SUM(Qty) as TotalDispatchedQty FROM dispatch WHERE flagdeleted = 0 GROUP BY PackingOrderId, palletno, unitid) d 
                                     ON p.PackingOrderId = d.PackingOrderId AND p.Palletno = d.palletno AND p.unitid = d.unitid
                                     LEFT JOIN PARTY C ON p.PARTYID = C.PARTYID
                                     LEFT JOIN itemgrades ig ON p.gradeid = ig.id
                                     LEFT JOIN prodqaul pq ON p.brandid = pq.id
                                     LEFT JOIN Recipe rc ON p.RecipeId = rc.Recipeid
                                     LEFT JOIN location_master lm ON p.LocationId = lm.id 
                                     LEFT JOIN users ur ON p.userid = ur.userid
                                     WHERE p.unitid = @Unit and p.approved=1 AND p.flagdeleted = 0
                                     AND (@Locate IS NULL OR lm.name = @Locate)
                                     AND (@Packingorderno IS NULL OR p.PackingOrderno = @Packingorderno)
                                     AND p.PackingOrderDate >= @PackingDateFrom AND p.PackingOrderDate <= @PackingDate
                                     GROUP BY p.PackingOrderno, p.palletIrn, lm.name, ur.username, p.draftid, p.PackingOrderId, c.partytypeid + '-' + c.partyname, 
                                     p.Palletno, p.marka, ig.gradename, pq.name, rc.recipename, p.qty, COALESCE(d.TotalDispatchedQty, 0)
                                     HAVING (p.Qty - COALESCE(d.TotalDispatchedQty, 0)) > 0
                                     ORDER BY p.PackingOrderId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Packingorderno", packgrower ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Locate", Tlocation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Unit", unit);
                    cmd.Parameters.AddWithValue("@PackingDateFrom", EditModel.PackingOrderDatefrom);
                    cmd.Parameters.AddWithValue("@PackingDate", EditModel.PackingOrderDateto);
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            GetPackingStock.Add(new CompanyModel
                            {
                                PalletId = Convert.ToInt32(rdr["palletno"]),
                                PackingOrderIrn = rdr["PackingOrderno"].ToString(),
                                GrowerGroupName = rdr["PartyName"].ToString(),
                                PalletName = rdr["palletIrn"].ToString(),
                                PackingQty = Convert.ToInt32(rdr["PackingQty"]),
                                DispatchQty = Convert.ToInt32(rdr["TotalDispatchedQty"]),
                                Packingbalance = Convert.ToInt32(rdr["AvailableQty"]),
                                ActualPackingbalance = Convert.ToInt32(rdr["AvailableQty"]),
                                VarietyId = rdr["variety"].ToString(),
                                ReceipeName = rdr["recipename"].ToString(),
                                ReceipeGrade = rdr["gradename"].ToString(),
                                ReceipeMarka = rdr["marka"].ToString(),
                                UserName = rdr["username"].ToString(),
                                ReceipeLocation = rdr["name"].ToString(),
                                Packingorderid = Convert.ToInt32(rdr["PackingOrderId"]),
                            });
                        }
                    }
                }
            }
            return GetPackingStock;
        }

        public async Task<List<CompanyModel>> GenerateDispatchReportComplete(string packgrower, string Tlocation, int unit, CompanyModel EditModel)
        {
            List<CompanyModel> GetPackingStock = new List<CompanyModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT d.dispatcherid, p.draftid, p.palletno, p.PackingOrderId, p.PackingOrderno, p.palletIrn, lm.name, ur.username,
                                     c.partytypeid + '-' + c.partyname as PartyName, p.Palletno, p.marka, ig.gradename, pq.name as variety,
                                     rc.recipename, p.qty AS PackingQty, COALESCE(d.TotalDispatchedQty, 0) AS TotalDispatchedQty,
                                     (p.Qty - COALESCE(d.TotalDispatchedQty, 0)) AS AvailableQty
                                     FROM packingorderitems p
                                     LEFT JOIN (SELECT dispatcherid, PackingOrderId, palletno, unitid, SUM(Qty) as TotalDispatchedQty FROM dispatch WHERE flagdeleted = 0 GROUP BY PackingOrderId, palletno, unitid, dispatcherid) d 
                                     ON p.PackingOrderId = d.PackingOrderId AND p.Palletno = d.palletno AND p.unitid = d.unitid
                                     LEFT JOIN PARTY C ON p.PARTYID = C.PARTYID
                                     LEFT JOIN itemgrades ig ON p.gradeid = ig.id
                                     LEFT JOIN prodqaul pq ON p.brandid = pq.id
                                     LEFT JOIN Recipe rc ON p.RecipeId = rc.Recipeid
                                     LEFT JOIN location_master lm ON p.LocationId = lm.id 
                                     LEFT JOIN users ur ON p.userid = ur.userid
                                     WHERE p.unitid = @Unit and p.approved=1 AND p.flagdeleted = 0
                                     AND (@Locate IS NULL OR lm.name = @Locate)
                                     AND (@Packingorderno IS NULL OR p.PackingOrderno = @Packingorderno)
                                     AND p.PackingOrderDate >= @PackingDateFrom AND p.PackingOrderDate <= @PackingDate
                                     GROUP BY p.PackingOrderno, p.palletIrn, lm.name, ur.username, d.dispatcherid, p.draftid, p.PackingOrderId, c.partytypeid + '-' + c.partyname, 
                                     p.Palletno, p.marka, ig.gradename, pq.name, rc.recipename, p.qty, COALESCE(d.TotalDispatchedQty, 0)
                                     HAVING (p.Qty - COALESCE(d.TotalDispatchedQty, 0)) = 0
                                     ORDER BY p.PackingOrderId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Packingorderno", packgrower ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Locate", Tlocation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Unit", unit);
                    cmd.Parameters.AddWithValue("@PackingDateFrom", EditModel.PackingOrderDatefrom);
                    cmd.Parameters.AddWithValue("@PackingDate", EditModel.PackingOrderDateto);
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            GetPackingStock.Add(new CompanyModel
                            {
                                DispatcherId = Convert.ToInt32(rdr["dispatcherid"]),
                                PalletId = Convert.ToInt32(rdr["palletno"]),
                                PackingOrderIrn = rdr["PackingOrderno"].ToString(),
                                GrowerGroupName = rdr["PartyName"].ToString(),
                                PalletName = rdr["palletIrn"].ToString(),
                                PackingQty = Convert.ToInt32(rdr["PackingQty"]),
                                DispatchQty = Convert.ToInt32(rdr["TotalDispatchedQty"]),
                                Packingbalance = Convert.ToInt32(rdr["AvailableQty"]),
                                ActualPackingbalance = Convert.ToInt32(rdr["AvailableQty"]),
                                VarietyId = rdr["variety"].ToString(),
                                ReceipeName = rdr["recipename"].ToString(),
                                ReceipeGrade = rdr["gradename"].ToString(),
                                ReceipeMarka = rdr["marka"].ToString(),
                                UserName = rdr["username"].ToString(),
                                ReceipeLocation = rdr["name"].ToString(),
                                Packingorderid = Convert.ToInt32(rdr["PackingOrderId"]),
                            });
                        }
                    }
                }
            }
            return GetPackingStock;
        }

        public async Task<CompanyModel?> ValidatePelletDispatch(CompanyModel companyModel, int unit)
        {
            if (companyModel == null) return null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("ValidatePelletDispatch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OutID", companyModel.PalletId);
                    cmd.Parameters.AddWithValue("@NewQty", companyModel.DispatchQty);
                    cmd.Parameters.AddWithValue("@unitid", unit);
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

        public async Task<CompanyModel?> AddFinalDispatch(CompanyModel EditModel, int unit)
        {
            if (EditModel == null) return null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("AddFinalDispatch", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PalletId", EditModel.PalletId);
                            cmd.Parameters.AddWithValue("@PackingQty", EditModel.DispatchQty);
                            cmd.Parameters.AddWithValue("@Createdby", EditModel.GlobalUserName);
                            cmd.Parameters.AddWithValue("@Unitid", unit);
                            cmd.Parameters.AddWithValue("@PackingOrderId", EditModel.Packingorderid);
                            await cmd.ExecuteNonQueryAsync();
                        }
                        using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con, transaction))
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
                        transaction.Commit();
                        return EditModel;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        EditModel.RetMessage = "Error: " + ex.Message;
                        EditModel.RetFlag = "FALSE";
                        return EditModel;
                    }
                }
            }
        }

        public async Task<CompanyModel?> Deletedispatch(int id, CompanyModel companyModel, int unit)
        {
            if (companyModel == null) return null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("Deletedispatch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mid", id);
                    cmd.Parameters.AddWithValue("@unitid", unit);
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

        public async Task<List<CompanyModel>> GenerateDispatchReportpend(int unit, CompanyModel EditModel)
        {
            List<CompanyModel> GetPackingStock = new List<CompanyModel>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT p.draftid, p.palletno, p.PackingOrderId, p.PackingOrderno, p.palletIrn, lm.name, ur.username,
                                     c.partytypeid + '-' + c.partyname as PartyName, p.Palletno, p.marka, ig.gradename, pq.name as variety,
                                     rc.recipename, p.qty AS PackingQty, COALESCE(d.TotalDispatchedQty, 0) AS TotalDispatchedQty,
                                     (p.Qty - COALESCE(d.TotalDispatchedQty, 0)) AS AvailableQty
                                     FROM packingorderitems p
                                     LEFT JOIN (SELECT PackingOrderId, palletno, unitid, SUM(Qty) as TotalDispatchedQty FROM dispatch WHERE flagdeleted = 0 GROUP BY PackingOrderId, palletno, unitid) d 
                                     ON p.PackingOrderId = d.PackingOrderId AND p.Palletno = d.palletno AND p.unitid = d.unitid
                                     LEFT JOIN PARTY C ON p.PARTYID = C.PARTYID
                                     LEFT JOIN itemgrades ig ON p.gradeid = ig.id
                                     LEFT JOIN prodqaul pq ON p.brandid = pq.id
                                     LEFT JOIN Recipe rc ON p.RecipeId = rc.Recipeid
                                     LEFT JOIN location_master lm ON p.LocationId = lm.id 
                                     LEFT JOIN users ur ON p.userid = ur.userid
                                     WHERE p.unitid = @Unit and p.approved=1 AND p.flagdeleted = 0
                                     GROUP BY p.PackingOrderno, p.palletIrn, lm.name, ur.username, p.draftid, p.PackingOrderId, c.partytypeid + '-' + c.partyname, 
                                     p.Palletno, p.marka, ig.gradename, pq.name, rc.recipename, p.qty, COALESCE(d.TotalDispatchedQty, 0)
                                     HAVING (p.Qty - COALESCE(d.TotalDispatchedQty, 0)) > 0
                                     ORDER BY p.PackingOrderId, p.palletIrn";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Unit", unit);
                    await con.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            GetPackingStock.Add(new CompanyModel
                            {
                                PalletId = Convert.ToInt32(rdr["palletno"]),
                                PackingOrderIrn = rdr["PackingOrderno"].ToString(),
                                GrowerGroupName = rdr["PartyName"].ToString(),
                                PalletName = rdr["palletIrn"].ToString(),
                                PackingQty = Convert.ToInt32(rdr["PackingQty"]),
                                DispatchQty = Convert.ToInt32(rdr["TotalDispatchedQty"]),
                                Packingbalance = Convert.ToInt32(rdr["AvailableQty"]),
                                ActualPackingbalance = Convert.ToInt32(rdr["AvailableQty"]),
                                VarietyId = rdr["variety"].ToString(),
                                ReceipeName = rdr["recipename"].ToString(),
                                ReceipeGrade = rdr["gradename"].ToString(),
                                ReceipeMarka = rdr["marka"].ToString(),
                                UserName = rdr["username"].ToString(),
                                ReceipeLocation = rdr["name"].ToString(),
                                Packingorderid = Convert.ToInt32(rdr["PackingOrderId"]),
                            });
                        }
                    }
                }
            }
            return GetPackingStock;
        }
    }
}
