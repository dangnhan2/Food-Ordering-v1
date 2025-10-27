using FoodOrdering.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
        public int SoldQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        public MenuDto() { }
        public MenuDto(Menus menu)
        {
            Id = menu.Id;
            Name = menu.Name;
            Category = menu.Categories.Name;
            Description = menu.Description;
            Price = menu.Price;
            ImageUrl = menu.ImageUrl;
            SoldQuantity = menu.SoldQuantity;
            IsAvailable = menu.IsAvailable;
            CreatedAt = menu.CreatedAt;
        }
    }
}
