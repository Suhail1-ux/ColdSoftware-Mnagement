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
    public class PackingDraftService : IPackingDraftService
    {
        private readonly IConfiguration _configuration;

        public PackingDraftService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CompanyModel?> ValidateDraftQty(CompanyModel companyModel)
        {
            if (companyModel == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("ValidateDraftQty", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OutID", companyModel.DemandNo);
                    cmd.Parameters.AddWithValue("@Lotno", companyModel.Lotno);
                    cmd.Parameters.AddWithValue("@NewQty", companyModel.GradingQty);
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

        public async Task<List<CompanyModel>> GetallDraftno(int Unit)
        {
            List<CompanyModel> PackingOrderist = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT DISTINCT draftirn from DraftOutTrans 
where unitid=@Unitid and Draftstat=@draftstat";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Unitid", Unit);
                cmd.Parameters.AddWithValue("@draftstat", "Open");
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        DraftIrn = rdr["draftirn"].ToString()
                    };
                    PackingOrderist.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return PackingOrderist;
        }

        public async Task<List<CompanyModel>> GetallDraftnobill(int Unit)
        {
            List<CompanyModel> PackingOrderist = new List<CompanyModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                const string query = @"SELECT DISTINCT draftirn from DraftOutTrans 
where Draftstat=@draftstat and flagdeleted=0 and draftirn not in(Select  draftirn from GradingPackingfinal)";
                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Unitid", Unit);
                cmd.Parameters.AddWithValue("@draftstat", "Closed");
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CompanyModel companyModel = new CompanyModel
                    {
                        DraftIrn = rdr["draftirn"].ToString()
                    };
                    PackingOrderist.Add(companyModel);
                }
                con.Close();
                cmd.Dispose();
            }
            return PackingOrderist;
        }

        public async Task<CompanyModel?> AddFinalDraft(CompanyModel EditModel)
        {
            if (EditModel == null) return null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("AddFinalDraft", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@tempid", EditModel.TempDraftId);
                            cmd.Parameters.AddWithValue("@Dated", EditModel.DraftDate);
                            cmd.Parameters.AddWithValue("@Createdby", EditModel.GlobalUserName);
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
                        EditModel.RetMessage = $"Error: {ex.Message}";
                        EditModel.RetFlag = "FALSE";
                        return EditModel;
                    }
                }
            }
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
    }
}
