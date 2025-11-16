using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FoodOrdering.Infrastructure.SignalR_Hub
{
    public sealed class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            // get role of user from claims
            var roles = Context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            // check role if user is admin
            if (roles.Contains("Admin"))
            // if user is admin to admin group
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User: {userId}");
        }
      
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
