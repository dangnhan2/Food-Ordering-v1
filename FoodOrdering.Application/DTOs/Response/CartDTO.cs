using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class CartDTO
    {
        public Guid Id { set; get; }
        public Guid UserId { get; set; }
        public ICollection<CartItemDTO> Items { get; set; }  = new List<CartItemDTO>();
    }
}
