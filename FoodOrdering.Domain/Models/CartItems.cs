using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class CartItems
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Carts Cart { get; set; } = null!;
        public Guid MenuId { get; set; }
        public Menus Menu { get; set; } = null!;
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
