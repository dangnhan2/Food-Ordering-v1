using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.DTOs.Response
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public double AverageRating { get; set; }
        public string ImageUrl { get; set; }
        public int SoldQuantity { get; set; }
        public int RatingCount { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime CreatedAt { get; set; }

        public MenuDto() { }
    }
}
