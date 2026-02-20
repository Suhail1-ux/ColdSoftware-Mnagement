
namespace ColdStoreManagement.BLL.Models.Chamber
{
    public class GrowerModel
    {
        public int Growerid { get; set; }

        public string? GrowerAddress { get; set; }
        public string? GrowerName { get; set; }
        public string? GrowerGroupName { get; set; }
        
        public string? OutwardGrowerGroup { get; set; }
        
        /// <summary>
        /// Maps to 'createdon' from the database
        /// </summary>
        public string? Cdated { get; set; }

        /// <summary>
        /// Maps to 'status' from the database
        /// </summary>
        public string? Grstatus { get; set; }
    }
}
