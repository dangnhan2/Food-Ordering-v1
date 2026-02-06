using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class VoucherValidationDto
    {
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public VoucherValidationDto(decimal discountAmount, decimal totalAmount)
        {
            DiscountAmount = discountAmount;
            TotalAmount = totalAmount;
        }
    }
}
