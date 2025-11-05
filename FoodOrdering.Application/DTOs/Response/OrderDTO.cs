using Food_Ordering.Models.Enum;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.DTOs.Response
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int TotalAmount { get; set; }
        public int TransactionCode { get; set; }
        public ICollection<OrderMenuDTO> Menus { get; set; } = new List<OrderMenuDTO>();

        public OrderDTO() { }
        public OrderDTO(Orders order, List<OrderMenuDTO> menus) 
        {
            Id = order.Id;
            UserId = order.UserId;
            OrderDate = order.OrderDate;
            FullName = order.Address.FullName;
            PhoneNumber = order.Address.PhoneNumber;
            Address = order.Address.Address;
            OrderStatus = order.Status;
            TotalAmount = order.TotalAmount;
            TransactionCode = order.TransactionId;
            Menus = menus;
        }
    }
}
