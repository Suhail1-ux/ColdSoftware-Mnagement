namespace ColdStoreManagement.BLL.Data
{
    public class SubGrower
    {
        public int Id { get; set; }
        public string SubGrowerName { get; set; }

        // Foreign Key
        public int? partyid { get; set; }

        // Navigation property: each subgrower belongs to one grower
        public Party party { get; set; }
    }

}
