

namespace ColdStoreManagement.BLL.Models
{
    public class UserPrivModel
    {
        public int GroupId { get; set; }
        public string? Pname { get; set; }
        public bool PageView { get; set; }
        public bool AddVal { get; set; }
        public bool EditVal { get; set; }
        public bool ViewVal { get; set; }
        public bool DelVal { get; set; }
        public bool Approval { get; set; }

        // Optional: Helper to check if the user has any permissions at all
        // public bool HasAnyAccess => AddVal || EditVal || ViewVal || DelVal;
    }
}
