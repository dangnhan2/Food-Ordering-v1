using Food_Ordering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class Menus
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Categories Categories { get; set; } = null!;
        public Guid CategoriesId { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderMenus> OrderMenus { get; set; } = new List<OrderMenus>();
        public ICollection<CartItems> CartItems { get;set; } = new List<CartItems>();
    }
}
