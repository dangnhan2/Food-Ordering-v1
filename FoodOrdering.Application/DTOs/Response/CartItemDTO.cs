using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class CartItemDTO
    {
        public Guid Id { get; set; }
        public Guid MenuId { get;set; }
        public string MenuName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
