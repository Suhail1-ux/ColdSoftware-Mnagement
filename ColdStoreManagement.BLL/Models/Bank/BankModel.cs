namespace ColdStoreManagement.BLL.Models.Bank
{
    public class BankModel
    {
        public int? Bid { get; set; }
        public string? Bname { get; set; }
        public string? BankName { get; set; }
        
        //[Required(ErrorMessage = "Address is required")]
        public string? Caddress { get; set; }
        public string? Branch { get; set; }
        public string? Ifsc { get; set; }
        public string? Accname { get; set; }
        public string? Accno { get; set; }

    }
}
