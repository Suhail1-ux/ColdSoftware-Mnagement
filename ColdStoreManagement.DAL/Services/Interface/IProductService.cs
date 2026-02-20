using ColdStoreManagement.BLL.Models.Product;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IProductService
    {
        Task<bool> DoesProductExistAsync(string Prodname);
        Task<List<ProductModel>> GetallProducts();
        Task<ProductModel?> GetByIdAsync(int id);
        Task<bool> AddProduct(ProductModel productModel);
        Task<bool> UpdateProduct(int id, ProductModel productModel);
        Task<bool> DeleteProduct(int id);

    }
}
