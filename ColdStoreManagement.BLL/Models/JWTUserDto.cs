
namespace ColdStoreManagement.BLL.Models
{
    public class JWTUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int UnitId { get; set; }
        public string UnitName { get; set; } = null!;        
        public string Jti { get; set; } = null!;
    }
}
