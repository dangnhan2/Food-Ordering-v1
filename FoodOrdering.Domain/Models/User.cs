using Food_Ordering.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Carts Carts { get; set; }
        public RefreshTokens RefreshTokens { get; set; }
        public ICollection<VoucherRedemptions> VoucherRedemptions { get; set; } = new List<VoucherRedemptions>();
    }
}
