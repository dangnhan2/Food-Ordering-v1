using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class PaymentOrderInfo
    {
        public string CheckoutUrl { get; set; }
        public int OrderCode { get; set; }
    }
}
