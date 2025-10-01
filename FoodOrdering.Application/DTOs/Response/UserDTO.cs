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
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }
}
