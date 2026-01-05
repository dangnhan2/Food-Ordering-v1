using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class NotificationRepo : GenericRepo<Notification>, INotificationRepo
    {
        private readonly FoodOrderingDbContext _context;
        public NotificationRepo(FoodOrderingDbContext context): base(context) {
            _context = context;
        }

        public void DeleteListOfNotification(List<Guid> notificationIds)
        {
            var notifications = _context.Notification
                 .Where(n => notificationIds.Contains(n.Id));

            _context.Notification.RemoveRange(notifications);
        }
    }
}
