using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.DTOs.Response
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public int OriginalPrice { get; set; }
        public int? DiscountPrice { get; set; }
        public double AverageRating { get; set; }
        public string ImageUrl { get; set; }
        public int SoldQuantity { get; set; }
        public int RatingCount { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime CreatedAt { get; set; }

        public MenuDto() { }
        public MenuDto(Menu menu, int ratingCount)
        {
            Id = menu.Id;
            Name = menu.Name;
            Category = menu.Categories.Name;
            Description = menu.Description;
            OriginalPrice = menu.OriginalPrice;
            AverageRating = menu.AverageRating;
            DiscountPrice = menu.DiscountPrice;
            ImageUrl = menu.ImageUrl;
            SoldQuantity = menu.SoldQuantity;
            RatingCount = ratingCount;
            IsAvailable = menu.IsAvailable;
            IsOnSale = menu.IsOnSale;
            CreatedAt = menu.CreatedAt;
        }
    }
}
