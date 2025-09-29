using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.QueryParams
{
    public class MenuParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
