using Food_Ordering.Models;
using Food_Ordering.Models.Enum;

namespace Food_Ordering.DTOs.Response
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public OrderStatus Status { get; set; }
        public double ToTalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int TransactionId { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();  
    }
}
