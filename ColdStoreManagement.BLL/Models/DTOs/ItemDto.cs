namespace ColdStoreManagement.BLL.Models.DTOs
{
    public class ItemDto
    {
        public string? PurchGrp { get; set; }
        public int Itemid { get; set; }
        public string? PurchaseItemName { get; set; }
        public string? ItemUom { get; set; }
        public string? ItemGst { get; set; }
        public string? ItemHsn { get; set; }
        public bool ItemStatus { get; set; }
        public DateTime ItemCreatedDate { get; set; }

        public string? RetFlag { get; set; }
        public string? RetMessage { get; set; }
    }
}
