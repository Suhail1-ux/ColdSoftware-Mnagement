
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Crate
{
    public class CrateIssue
    {
        public int Cid { get; set; } // Primary Key
        public DateTime? Dated { get; set; }
        public int? Partyid { get; set; }
        public int? GroowerId { get; set; }
        public int? ChallanId { get; set; }

        public int? Createdby { get; set; }
        public DateTime? Createdon { get; set; }
        public string? CrateMark { get; set; }

        public int? Updatedby { get; set; }
        public DateTime? Updatedon { get; set; }

        public int? Vehid { get; set; } // Vehicle ID
        public string? Remarks { get; set; }
        public int? Flagdeleted { get; set; }
        public decimal? Qty { get; set; }
        [StringLength(30)]
        public string Trflag { get; set; } = string.Empty; // Primary Key (varchar 30)
        public string? Trno { get; set; }
    }
}
