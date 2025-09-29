using FluentValidation;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class MenuRequest
    {   
        public IFormFile ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public Guid CategoriesId { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public bool IsAvailble { get; set; }
        public int StockQuantity { get; set; }
    }
}
