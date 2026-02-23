using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Product
{
    public class ProductQualityModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
