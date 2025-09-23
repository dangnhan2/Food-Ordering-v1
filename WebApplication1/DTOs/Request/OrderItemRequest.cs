using Food_Ordering.Models;

namespace Food_Ordering.DTOs.Request
{
    public class OrderItemRequest
    {
        public Guid MenuItemsId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double SubTotal { get; set; }
    }
}
