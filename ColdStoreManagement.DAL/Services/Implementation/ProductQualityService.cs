using ColdStoreManagement.BLL.Errors;
using ColdStoreManagement.BLL.Models.Product;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class ProductQualityService(SQLHelperCore sql) : BaseService(sql), IProductQualityService
    {
        public async Task<bool> DoesQualityExistAsync(string name)
        {
            int count = await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM prodqaul WHERE Name = @Name",
                CommandType.Text,
                new SqlParameter("@Name", name));

            return count > 0;
        }

        public async Task<List<ProductQualityModel>> GetAllQualitiesAsync()
        {
            return await _sql.ExecuteReaderAsync<ProductQualityModel>(
                @"SELECT id as Id, 
                         name as Name, 
                         qdescrip as Description 
                  FROM dbo.prodqaul",
                CommandType.Text
            );
        }

        public async Task<ProductQualityModel?> GetByIdAsync(int id)
        {
            return await _sql.ExecuteSingleAsync<ProductQualityModel>(
                @"SELECT id as Id, 
                         name as Name, 
                         qdescrip as Description 
                  FROM dbo.prodqaul 
                  WHERE id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id)
            );
        }

        public async Task<bool> AddQualityAsync(ProductQualityModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
                INSERT INTO dbo.prodqaul (id, name, qdescrip) 
                VALUES ((SELECT ISNULL(MAX(id) + 1, 1) FROM prodqaul), @Name, @Description)";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Description", model.Description ?? (object)DBNull.Value)
            );

            return rows > 0;
        }

        public async Task<bool> UpdateQualityAsync(int id, ProductQualityModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            const string query = @"
                UPDATE dbo.prodqaul 
                SET name = @Name, 
                    qdescrip = @Description 
                WHERE id = @Id";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Description", model.Description ?? (object)DBNull.Value)
            );

            if (rows == 0)
                throw new NotFoundException("Product quality not found");

            return true;
        }

        public async Task<bool> DeleteQualityAsync(int id)
        {
            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE FROM dbo.prodqaul WHERE id = @Id",
                new SqlParameter("@Id", id)
            );

            return rows > 0;
        }
    }
}
