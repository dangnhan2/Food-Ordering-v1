using Microsoft.AspNetCore.Http;

namespace FoodOrdering.Application.DTOs.Request
{
    public class MenuRequestDto
    {   
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public int OriginalPrice { get; set; }
        public int? DiscountPrice { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOnSale { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
