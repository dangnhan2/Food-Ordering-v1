using Food_Ordering.Models.Enum;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.DTOs.Response
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OrderDate { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Note { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderCode { get; set; }     
        public string PaymentMethod { get; set; }
        public ICollection<OrderMenuDTO> Menus { get; set; } = new List<OrderMenuDTO>();

    }
}
