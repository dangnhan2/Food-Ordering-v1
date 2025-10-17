using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class NotificationDto
    {  
        public Guid Id { get; set; }
        public string Tiltle { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
