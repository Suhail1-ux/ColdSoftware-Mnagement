using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Product
{
    public class ProductModel
    {   
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; } = null!;
        public string? ProductDetails { get; set; }

    }
}
