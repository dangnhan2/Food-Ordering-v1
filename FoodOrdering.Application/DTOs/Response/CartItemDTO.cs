using FoodOrdering.Domain.Models;
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
        public decimal UnitPrice { get; set; }

        public CartItemDTO() { }
        public CartItemDTO(CartItem item) {
            Id = item.Id;
            MenuId = item.MenuId;
            MenuName = item.Menu.Name;
            ImageUrl = item.Menu.ImageUrl;
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
        }
    }
}
