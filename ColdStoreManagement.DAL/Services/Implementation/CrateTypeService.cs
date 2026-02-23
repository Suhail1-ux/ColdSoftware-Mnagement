using ColdStoreManagement.BLL.Models.Crate;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class CrateTypeService : BaseService, ICrateTypeService
    {
        public CrateTypeService(SQLHelperCore sql) : base(sql) { }

        public async Task<List<CrateType>> GetAllCrateTypesAsync()
        {
            return await _sql.ExecuteReaderAsync<CrateType>("SELECT id, name, Crqty FROM crtypes ORDER BY id", CommandType.Text);
        }

        public async Task<bool> DoesCrateTypeExistAsync(string name)
            => (await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM crtypes WHERE name=@Name",
                CommandType.Text,
                new SqlParameter("@Name", name))) > 0;

        public async Task<bool> AddCrateTypeAsync(CrateType model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"insert into dbo.crtypes (id,name,Crqty) 
                  values((select isnull(max(id+1),1) from crtypes),@Crname,@Crqty)",
                new SqlParameter("@Crname", model.Name),
                new SqlParameter("@Crqty", model.Crqty));

            return true;
        }

        public async Task<bool> UpdateCrateTypeAsync(CrateType model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "update dbo.crtypes set name=@Crname,Crqty=@Crqty where id=@Id",
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Crname", model.Name),
                new SqlParameter("@Crqty", model.Crqty));
            return true;
        }

        public async Task<bool> DeleteCrateTypeAsync(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE FROM crtypes WHERE id=@id",
                new SqlParameter("@id", id));
            return true;
        }
    }
}
