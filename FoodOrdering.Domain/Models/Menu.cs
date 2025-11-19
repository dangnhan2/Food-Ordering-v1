namespace FoodOrdering.Domain.Models
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CategoriesId { get; set; }
        public Category Categories { get; set; } 

        public string? Description { get; set; }
        public int OriginalPrice { get; set; }
        public int? DiscountPrice { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOnSale { get; set; }
        public double AverageRating { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderMenus> OrderMenus { get; set; } = new List<OrderMenus>();
        public ICollection<CartItem> CartItems { get;set; } = new List<CartItem>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
