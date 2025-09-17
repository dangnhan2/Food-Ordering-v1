using Food_Ordering.Models;

namespace Food_Ordering.DTOs.Request
{
    public class MenuItemRequest
    {
        public string Name { get; set; }
        public Guid MenuCategoriesId { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public IFormFile? File { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
    }
}
