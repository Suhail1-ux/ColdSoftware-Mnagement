using ColdStoreManagement.BLL.Errors;
using ColdStoreManagement.BLL.Models.Product;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class ProductService(SQLHelperCore sql) : BaseService(sql), IProductService
    {
        public async Task<bool> DoesProductExistAsync(string Prodname)
        {
            int count = await _sql.ExecuteScalarAsync<int>(
              "SELECT COUNT(1) FROM prodtype WHERE Name = @Name",
              CommandType.Text,
              new SqlParameter("@Name", Prodname));

            return (count > 0);
        }
        public async Task<List<ProductModel>> GetallProducts()
        {
            return await _sql.ExecuteReaderAsync<ProductModel>(
               @"select id as ProductId,	
                name as ProductName,
                prodetails as ProductDetails
                from dbo.prodtype",
               CommandType.Text
            );
        }
        public async Task<ProductModel?> GetByIdAsync(int id)
        {
            return await _sql.ExecuteSingleAsync<ProductModel>(
               @"select id as ProductId,	
                name as ProductName,
                prodetails as ProductDetails
                from dbo.prodtype where Id=@Id",
               CommandType.Text,
               new SqlParameter("@Id", id)
            );
        }
        public async Task<bool> AddProduct(ProductModel productModel)
        {
            if (productModel == null)
                throw new ArgumentNullException(nameof(productModel));

            const string query = @"
                INSERT INTO dbo.prodtype (id, name, prodetails)
                VALUES (
                    (SELECT ISNULL(MAX(id) + 1, 1) FROM prodtype),
                    @Prodname,
                    @Prodetails
                )";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Prodname", productModel.ProductName),
                new SqlParameter("@Prodetails", productModel.ProductDetails)
            );

            return rows > 0;
        }
        public async Task<bool> UpdateProduct(int id, ProductModel productModel)
        {
            if (productModel == null)
                throw new ArgumentNullException(nameof(productModel));

            const string query = @"
                UPDATE dbo.prodtype
                SET name = @Prodname,
                    prodetails = @Prodetails
                WHERE id = @Id";

            var rows = await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@Id", id),
                new SqlParameter("@Prodname", productModel.ProductName),
                new SqlParameter("@Prodetails", productModel.ProductDetails)
            );

            if (rows == 0)
                throw new NotFoundException("Product not found");

            return true;
        }
        public async Task<bool> DeleteProduct(int id)
        {
            await _sql.ExecuteNonQueryAsync(
               CommandType.Text,
               "delete dbo.Prodtype where Id=@Id",
               new SqlParameter("@Id", id)
           );
            return true;
        }

    }
}
