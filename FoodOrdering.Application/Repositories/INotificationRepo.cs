using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.Repositories
{
    public interface INotificationRepo : IGenericRepo<Notification>
    {
        public void DeleteListOfNotification(List<Guid> notificationIds);
    }
}
