using Food_Ordering.Models;

namespace Food_Ordering.DTOs.Response
{
    public class OrderItemDto
    {
        public Guid MenuItemsId { get; set; }
        public string? ItemName { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double SubTotal { get; set; }
    }
}
