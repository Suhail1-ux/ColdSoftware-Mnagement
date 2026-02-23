using ColdStoreManagement.BLL.Models.Crate;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class CrateFlagService : BaseService, ICrateFlagService
    {
        public CrateFlagService(SQLHelperCore sql) : base(sql) { }

        public async Task<List<CrateFlags>> GetAllCrateFlagsAsync()
        {
            return await _sql.ExecuteReaderAsync<CrateFlags>("SELECT id, name, ftype, srtype FROM CrateFlags ORDER BY id", CommandType.Text);
        }

        public async Task<bool> DoesCrateFlagExistAsync(string name)
            => (await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM CrateFlags WHERE name=@Name",
                CommandType.Text,
                new SqlParameter("@Name", name))) > 0;

        public async Task<bool> AddCrateFlagAsync(CrateFlags model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"insert into dbo.CrateFlags (id,name,ftype,srtype)  
                  values((select isnull(max(id+1),1) from CrateFlags),@Cfname,@Cfstat,@Cflag)",
                new SqlParameter("@Cfname", model.Name),
                new SqlParameter("@Cfstat", model.Ftype),
                new SqlParameter("@Cflag", model.Srtype));

            return true;
        }

        public async Task<bool> UpdateCrateFlagAsync(CrateFlags model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "update dbo.CrateFlags set name=@Cfname,ftype=@Cfstat,srtype=@Cflag where id=@Id",
                new SqlParameter("@id", model.Id),
                new SqlParameter("@Cfname", model.Name),
                new SqlParameter("@Cfstat", model.Ftype),
                new SqlParameter("@Cflag", model.Srtype));

            return true;
        }

        public async Task<bool> DeleteCrateFlagAsync(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE FROM CrateFlags WHERE id=@id",
                new SqlParameter("@id", id));

            return true;
        }
    }
}
