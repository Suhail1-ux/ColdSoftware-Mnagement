
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Crate
{
    public class CrateFlags
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [StringLength(20)]
        public string? Ftype { get; set; } // varchar(20)
        [StringLength(30)]
        public string? Srtype { get; set; } // varchar(30)
    }
    public class CrateType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Crqty { get; set; } // numeric(18,0)
    }
}
