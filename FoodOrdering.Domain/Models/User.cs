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
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
        public Cart Cart { get; set; }
        public ICollection<EmailOtp> EmailOtps { get; set; } = new List<EmailOtp>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<VoucherRedemptions> VoucherRedemptions { get; set; } = new List<VoucherRedemptions>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
