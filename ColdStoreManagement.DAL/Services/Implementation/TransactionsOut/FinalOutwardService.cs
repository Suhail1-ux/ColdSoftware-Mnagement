using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ColdStoreManagement.DAL.Services.Implementation.TransactionsOut
{
    public class FinalOutwardService : IFinalOutwardService
    {
        private readonly IConfiguration _configuration;

        public FinalOutwardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CompanyModel?> FInaloutwardPriv(string Ugroup)
        {
            CompanyModel? companyModel = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                const string query = "select Addval,Editval,ViewVal,DelVal,Approval from userpriv where Groupid in (select usergroupid from usergroup where name=@Ugroup) and pname=@pname";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Ugroup", Ugroup);
                    cmd.Parameters.AddWithValue("@pname", "Final OutWard");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            companyModel = new CompanyModel
                            {
                                FoAdd = reader.GetBoolean(reader.GetOrdinal("Addval")),
                                FoEdit = reader.GetBoolean(reader.GetOrdinal("Editval")),
                                FoView = reader.GetBoolean(reader.GetOrdinal("ViewVal")),
                                FoDel = reader.GetBoolean(reader.GetOrdinal("DelVal")),
                                FoApp = reader.GetBoolean(reader.GetOrdinal("Approval"))
                            };
                        }
                    }
                }
            }
            return companyModel;
        }

        public async Task<CompanyModel?> AddFinaloutward(CompanyModel EditModel, int unit)
        {
            if (EditModel == null) return null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("AddFinaloutward", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PalletId", EditModel.PalletId);
                            cmd.Parameters.AddWithValue("@PackingQty", EditModel.OutwardQty);
                            cmd.Parameters.AddWithValue("@Party", EditModel.OutwardGrowerGroup);
                            cmd.Parameters.AddWithValue("@Grower", EditModel.OutwardGrowerName);
                            cmd.Parameters.AddWithValue("@Challan", EditModel.ChallanName);
                            cmd.Parameters.AddWithValue("@Dated", EditModel.OutwardDate);
                            cmd.Parameters.AddWithValue("@Vehno", EditModel.Vehno);
                            cmd.Parameters.AddWithValue("@Createdby", EditModel.GlobalUserName);
                            cmd.Parameters.AddWithValue("@Unitid", unit);
                            cmd.Parameters.AddWithValue("@Recipeid", EditModel.ReceipeName);
                            cmd.Parameters.AddWithValue("@PackingOrderId", EditModel.Packingorderid);
                            cmd.Parameters.AddWithValue("@dispatch", EditModel.DispatcherId);
                            cmd.Parameters.AddWithValue("@Tempid", EditModel.TempOutwardId);
                            cmd.Parameters.AddWithValue("@Outfix", EditModel.OutwardFixid);
                            cmd.Parameters.AddWithValue("@Outwardno", EditModel.Outwardno);
                            cmd.Parameters.AddWithValue("@outwardtype", EditModel.Outwardtype);
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

        public async Task<CompanyModel?> UpdateFinaloutward(List<CompanyModel> checkedPellets, CompanyModel baseEditModel, int unit)
        {
            if (checkedPellets == null || !checkedPellets.Any() || baseEditModel == null) return null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlDbContext")))
            {
                await con.OpenAsync();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in checkedPellets)
                        {
                            using (SqlCommand cmd = new SqlCommand("updateFinaloutwardback", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 300;
                                cmd.Parameters.AddWithValue("@PalletId", item.PalletId);
                                cmd.Parameters.AddWithValue("@PackingQty", item.OutwardQty);
                                cmd.Parameters.AddWithValue("@Party", baseEditModel.OutwardGrowerGroup);
                                cmd.Parameters.AddWithValue("@Grower", baseEditModel.OutwardGrowerName);
                                cmd.Parameters.AddWithValue("@Challan", baseEditModel.ChallanName);
                                cmd.Parameters.AddWithValue("@Dated", baseEditModel.OutwardDate);
                                cmd.Parameters.AddWithValue("@Vehno", baseEditModel.Vehno);
                                cmd.Parameters.AddWithValue("@Tempid", baseEditModel.TempOutwardId);
                                cmd.Parameters.AddWithValue("@Createdby", baseEditModel.GlobalUserName);
                                cmd.Parameters.AddWithValue("@Unitid", unit);
                                cmd.Parameters.AddWithValue("@Recipeid", item.ReceipeName);
                                cmd.Parameters.AddWithValue("@PackingOrderId", baseEditModel.Packingorderid);
                                cmd.Parameters.AddWithValue("@dispatch", item.DispatcherId);
                                cmd.Parameters.AddWithValue("@Outfix", item.Outwardid);
                                cmd.Parameters.AddWithValue("@Outwardno", baseEditModel.Outwardno);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 flag,remarks FROM dbo.svalidate", con, transaction))
                        {
                            using (SqlDataReader rdr = await cmd2.ExecuteReaderAsync())
                            {
                                if (rdr.Read())
                                {
                                    baseEditModel.RetMessage = rdr["remarks"]?.ToString();
                                    baseEditModel.RetFlag = rdr["flag"]?.ToString();
                                }
                            }
                        }
                        transaction.Commit();
                        return baseEditModel;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        baseEditModel.RetMessage = "Error: " + ex.Message;
                        baseEditModel.RetFlag = "FALSE";
                        return baseEditModel;
                    }
                }
            }
        }
    }
}
