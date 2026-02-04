using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class UnreadNotificationDto
    {   
        public int Total { get; set; }
        public IEnumerable<NotificationDto> UnreadNotifications { get; set; } = new List<NotificationDto>();
    }
}
