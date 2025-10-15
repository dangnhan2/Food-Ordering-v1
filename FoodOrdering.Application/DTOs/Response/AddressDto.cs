using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
    }
}
