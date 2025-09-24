using Food_Ordering.Models;
using Food_Ordering.Models.Enum;

namespace Food_Ordering.DTOs.Request
{
    public class OrderRequest
    {
        public string UserId { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}
