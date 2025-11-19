using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;       
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
