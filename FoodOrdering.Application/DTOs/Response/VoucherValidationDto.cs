using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class VoucherValidationDto
    {
        public int DiscountAmount { get; set; }
        public int TotalAmount { get; set; }

        public VoucherValidationDto() { }
        public VoucherValidationDto(int discountAmount, int totalAmount)
        {
            DiscountAmount = discountAmount;
            TotalAmount = totalAmount;
        }
    }
}
