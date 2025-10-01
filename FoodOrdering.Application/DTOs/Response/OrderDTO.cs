using Food_Ordering.Models.Enum;
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
    }
}
