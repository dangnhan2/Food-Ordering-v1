using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class ValidateVoucherRequest
    {
        public Guid UserId { get; set; }
        public Guid VoucherId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
