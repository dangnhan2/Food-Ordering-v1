using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class OrderMenuDTO
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuImage { get; set; }
        public int Quantity { get; set; }
        public int SubPrice { get; set; }
        public bool IsRated { get; set; }
        public OrderMenuDTO() { }
        public OrderMenuDTO(OrderMenus order)
        {
            Id = order.Id;
            MenuId = order.MenuId;
            MenuName = order.Menus.Name;
            MenuImage = order.Menus.ImageUrl;
            Quantity = order.Quantity;
            SubPrice = order.UnitPrice * order.Quantity;
        }
    }
}
