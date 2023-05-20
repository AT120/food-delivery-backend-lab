using EasyNetQ;
using Microsoft.AspNetCore.SignalR;
using NotificationApi.Hubs;
using ProjCommon.Configurators;
using ProjCommon.DTO;
using ProjCommon.Enums;

namespace NotificationApi;

public class BackgroundNotifier : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hub;
    private readonly IBus _queue;
    private readonly ILogger _logger;

    public BackgroundNotifier(
        IHubContext<NotificationHub> hub,
        IBus queue,
        ILogger<string> logger)
    {
        _hub = hub;
        _queue = queue;
        _logger = logger;
    }

    private async Task MessageHandler(
        StatusChangedNotification notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(1, "NOTIFICATION SENDED TO {Id}", notification.UserId);
        await _hub.Clients.Users(notification.UserId.ToString()).SendAsync(
            "ReceiveMessage",
            $"Заказ {notification.OrderId} {OrderStatusNames.Names[notification.Status]}",
            cancellationToken
        );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(1, message: "RECIEVER HAVE BEEN SET UP");
        await _queue.SendReceive.ReceiveAsync<StatusChangedNotification>(
            NotificationQueueConfigurator.DefaultQueueName,
            MessageHandler,
            stoppingToken
        );
        // await _queue.PubSub.SubscribeAsync()
        
    }
}


// public class BackgroundNotifier2 : IHostedService
// {
//     private 
//     private readonly IHubContext<NotificationHub> _hub;
//     private readonly IBus _queue;

//     public BackgroundNotifier2(IHubContext<NotificationHub> hub, IBus queue)
//     {
//         _hub = hub;
//         _queue = queue;
//     }
//     public Task StartAsync(CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }

//     public Task StopAsync(CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
// }