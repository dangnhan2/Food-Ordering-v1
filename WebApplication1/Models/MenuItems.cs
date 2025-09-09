namespace Food_Ordering.Models
{
    public class MenuItems
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public MenuCategories MenuCategories { get; set; }
        public Guid MenuCategoriesId { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItems> Items { get; set; } = new List<OrderItems>();
    }
}
