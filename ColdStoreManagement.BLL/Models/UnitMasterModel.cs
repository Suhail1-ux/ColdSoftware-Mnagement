using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models
{
    public sealed class UnitMasterModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UnitCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UnitName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Stat { get; set; }
        public string? Details { get; set; }
    }

}
