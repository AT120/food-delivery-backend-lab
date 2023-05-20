using Microsoft.AspNetCore.SignalR;

namespace NotificationApi;

public class DefaultIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // В Name будет UserId
        return connection.User?.Identity?.Name; 
    }
}