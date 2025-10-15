using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class AddressRequest
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
    }
}
