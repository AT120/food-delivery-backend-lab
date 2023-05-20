using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace NotificationApi.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    // public async Task SendMessage()
    // {
    //     await Clients.All.SendAsync("ReceiveMessage", "Hallo");
    //     // ClaimTypes.NameIdentifier
    //     // return Task.CompletedTask;
    // }
}