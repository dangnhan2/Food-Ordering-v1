using Food_Ordering.Models;
using Food_Ordering.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class Orders
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Address { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public OrderStatus Status { get; set; }
        public int ToTalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int TransactionId { get; set; }
        public ICollection<OrderMenus> OrderMenus { get; set; } = new List<OrderMenus>();
    }
}
