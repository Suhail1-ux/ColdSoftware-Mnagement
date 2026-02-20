
namespace ColdStoreManagement.BLL.Models.Chamber
{
    public sealed class DashboardTotalsModel
    {
        //total_verified_qty
        public int TotalInQty { get; set; }
        //total_order_qty
        public int TotalOutQty { get; set; }
        public int TotalBalQty { get; set; }
    }

    public sealed class ChamberAllocationViewModel
    {
        public int ChamberId { get; set; }
        public string ChamberName { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public int Capacity { get; set; }
        public string? ChamberType { get; set; } 
        public int AllocatedQty { get; set; }
        public int RemainingQty { get; set; }
        
    }


}
