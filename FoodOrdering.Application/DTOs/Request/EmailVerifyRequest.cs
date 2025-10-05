using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class EmailVerifyRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
