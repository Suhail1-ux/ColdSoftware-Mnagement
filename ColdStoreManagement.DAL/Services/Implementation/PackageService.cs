using ColdStoreManagement.BLL.Errors;
using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class PackageService(SQLHelperCore sql) : BaseService(sql), IPackageService
    {
        public async Task<List<PackageModel>> GetAllPackagesAsync()
        {
            const string query = "SELECT id, name, pdescrip as Description FROM dbo.ptype";
            return await _sql.ExecuteReaderAsync<PackageModel>(
                query,
                CommandType.Text
            );
        }
        public async Task<PackageModel?> GetByIdAsync(int id)
        {
            return await _sql.ExecuteSingleAsync<PackageModel>(
                "SELECT id, name, pdescrip as Description FROM dbo.ptype WHERE id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id)
            );
        }
        public async Task<bool> AddPackageAsync(PackageModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
            INSERT INTO dbo.ptype (id, name, pdescrip)
            VALUES (
                (SELECT ISNULL(MAX(id) + 1, 1) FROM ptype),
                @Pkname,
                @Pkdetails
            )";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Pkname", model.Name),
                new SqlParameter("@Pkdetails", model.Description)
            );

            return rows > 0;
        }
        public async Task<bool> UpdatePackageAsync(int id, PackageModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
            UPDATE dbo.ptype
            SET name = @Pkname,
                pdescrip = @Pkdetails
            WHERE id = @Id";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Pkname", model.Name),
                new SqlParameter("@Pkdetails", model.Description)
            );

            if (rows == 0)
                throw new NotFoundException("Package not found");

            return true;
        }
        public async Task<bool> DeletePackageAsync(int id)
        {
            const string query = "DELETE dbo.ptype WHERE id = @Id";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id)
            );

            if (rows == 0)
                throw new NotFoundException("Package not found");

            return true;
        }

    }
}
