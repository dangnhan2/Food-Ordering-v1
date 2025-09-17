using Food_Ordering.Models;
using Food_Ordering.Models.Enum;

namespace Food_Ordering.DTOs.Response
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; }
        public double ToTalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItemDto> Items = new List<OrderItemDto>();  
    }
}
