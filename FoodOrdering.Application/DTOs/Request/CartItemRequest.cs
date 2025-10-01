using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class CartItemRequest
    {
        public Guid MenuId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
