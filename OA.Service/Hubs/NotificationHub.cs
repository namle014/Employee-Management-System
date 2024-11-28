using Microsoft.AspNetCore.SignalR;
using OA.Core.Services;

namespace Employee_Management_System.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            if (httpContext == null) return "CC001";

            var userId = httpContext.Request.Query["access_token"];
            return string.IsNullOrEmpty(userId) ? "CC001" : userId!;
        }
    }

    public class NotificationHub : Hub
    {
        private readonly IUserConnectionService _userConnectionService;

        public NotificationHub(IUserConnectionService userConnectionService)
        {
            _userConnectionService = userConnectionService;
        }

        // Khi người dùng kết nối, lưu ConnectionId với UserId
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await _userConnectionService.AddConnectionAsync(userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        // Khi người dùng ngắt kết nối, xóa ConnectionId khỏi danh sách
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await _userConnectionService.RemoveConnectionAsync(userId, Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }

}
