using System;

namespace ColdStoreManagement.BLL.Data
{
    public class Dtrans
    {
        public int Id { get; set; }

        public int chamberid { get; set; }

        public Chamber chamber { get; set; }

        public int partyid { get; set; }

        public Party party { get; set; }

        public int? SubGrowerId { get; set; }

        public SubGrower SubGrower { get; set; }

        public int AllocationId { get; set; }
        public Allocation Allocation { get; set; }

        public int? LotNumber { get;set; }

        public bool FlagDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
