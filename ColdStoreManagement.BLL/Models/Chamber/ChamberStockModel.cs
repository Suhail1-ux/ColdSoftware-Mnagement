
namespace ColdStoreManagement.BLL.Models.Chamber
{
    public sealed class ChamberStockModel
    {
        public int ChamberId { get; set; }
        public string ChamberName { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

        public int InQty { get; set; }
        public int OutQty { get; set; }
        public int ChamberBalance { get; set; }

        public string? ChamberinDate { get; set; }
        public string? ChamberOutDate { get; set; }
    }

}
