using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class CartRequestDto
    {
        public Guid UserId { get; set; }
        public ICollection<CartItemRequestDto> CartItems { get; set; } = new List<CartItemRequestDto>();
    }
}
