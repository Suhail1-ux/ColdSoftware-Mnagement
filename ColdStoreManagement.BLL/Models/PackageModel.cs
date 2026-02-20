using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models
{
    public class PackageModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Package name is required")]
        [StringLength(100, ErrorMessage = "Package name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
