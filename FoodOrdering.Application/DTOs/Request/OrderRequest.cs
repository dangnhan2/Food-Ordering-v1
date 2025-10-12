using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class OrderRequest
    {   
        public Guid UserId { get; set; }
        public Guid? VoucherId { get; set; } 
        public string Address { get; set; }
        public string? Note { get; set; }
        public int TotalAmount { get; set; }
    }
}
