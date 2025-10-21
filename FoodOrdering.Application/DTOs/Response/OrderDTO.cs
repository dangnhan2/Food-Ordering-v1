using Food_Ordering.Models.Enum;
using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int TotalAmount { get; set; }
        public ICollection<OrderMenuDTO> Menus { get; set; } = new List<OrderMenuDTO>();

        public OrderDTO() { }
        public OrderDTO(Orders order, List<OrderMenuDTO> menus) 
        {
            Id = order.Id;
            UserId = order.UserId;
            OrderDate = order.OrderDate;
            OrderStatus = order.Status;
            TotalAmount = order.ToTalAmount;
            Menus = menus;
        }
    }
}
