
namespace ColdStoreManagement.BLL.Models.Chamber
{
    public sealed class ChamberPartyStockModel
    {
        public int ChamberId { get; set; }
        public int PartyId { get; set; }
        public string ChamberName { get; set; } = string.Empty;
       // public string PartyName { get; set; } = string.Empty;
        public string GrowerGroupName { get; set; } = string.Empty;
        public int InQty { get; set; }
        public int OutQty { get; set; }
        public int BalanceQty { get; set; }
    }
    public sealed class ChamberGrowerStockModel
    {
        public int ChamberId { get; set; }
        public string ChamberName { get; set; } = string.Empty;
        public string GrowerName { get; set; } = string.Empty;
        public int InQty { get; set; }
        public int OutQty { get; set; }
        public int BalanceQty { get; set; }
    }

}
