using System;

namespace ColdStoreManagement.BLL.Data
{
    public class Allocation
    {
        public int Id { get; set; }

        public int chamberid { get; set; }

        public Chamber chamber { get; set; }

        public int partyid { get; set; }

        public Party party { get; set; }

        public int? SubGrowerId { get; set; }

        public SubGrower SubGrower { get; set; }

        public int Quantity { get; set; }
        public int? Series { get; set; }

        public bool FlagDeleted { get; set; }
        public bool FlagIsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
