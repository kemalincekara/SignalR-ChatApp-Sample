using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace ChatApp.Server.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            StringValues? userId = connection.GetHttpContext()?.Request?.Query["userId"];
            if (userId.HasValue && !StringValues.IsNullOrEmpty(userId.Value))
                return userId;
            else
                return connection.ConnectionId;
        }
    }
}