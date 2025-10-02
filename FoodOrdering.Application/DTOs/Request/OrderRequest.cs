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
        public string Address { get; set; }
        public string? Note { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int TransactionId { get; set; }
        public ICollection<OrderMenuRequest> Menus { get; set; } = new List<OrderMenuRequest>();
    }
}
