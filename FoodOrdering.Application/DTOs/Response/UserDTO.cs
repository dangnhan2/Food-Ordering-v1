using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class UserDTO
    {  
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalAmountInMonth { get; set; }
        public decimal TotalAmountInYear { get; set; }
        public string Role { get; set; }
    }
}
