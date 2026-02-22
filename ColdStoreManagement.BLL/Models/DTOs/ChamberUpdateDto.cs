namespace ColdStoreManagement.BLL.Models.DTOs
{
    public class ChamberUpdateDto
    {
        public int ChamberId { get; set; }
        public string? ChamberName { get; set; }
        public string? ChamberType { get; set; }
        public string? Unitname { get; set; }
        public string? Capacity { get; set; }
        public string? GlobalUserName { get; set; }

        public string? RetFlag { get; set; }
        public string? RetMessage { get; set; }
    }
}
