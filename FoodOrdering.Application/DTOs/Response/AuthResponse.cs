using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class AuthResponse
    {
        public UserDTO Data { get; set; }
        public string AccessToken { get; set; }
    }
}
