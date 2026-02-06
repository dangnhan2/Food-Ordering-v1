using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
     
    }
}
