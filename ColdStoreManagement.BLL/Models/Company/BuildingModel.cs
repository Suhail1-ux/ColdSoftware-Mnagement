using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdStoreManagement.BLL.Models.Company
{
    public class BuildingModel
    {
        public int Id { get; set; }
        public string Bcode { get; set; } = string.Empty;
        public string Buildname { get; set; } = string.Empty;
        public string Bstat { get; set; } = "Active";
        public string? BuildDetails { get; set; }

        // --- Helper Properties ---

        /// <summary>
        /// Returns true if the building status is active
        /// </summary>
        // public bool IsActive => Bstat?.Trim().ToUpper() == "ACTIVE" || Bstat?.Trim() == "1";
    }
}
