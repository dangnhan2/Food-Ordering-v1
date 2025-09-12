using Microsoft.AspNetCore.Identity;

namespace Food_Ordering.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public RefreshTokens RefreshTokens { get; set; }
        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}
