using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class TopDishDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int SoldQuantity { get; set; }

        public TopDishDto(Menus menu)
        {
            Name = menu.Name;
            ImageUrl = menu.ImageUrl;
            SoldQuantity = menu.SoldQuantity;
        }

    }
}
