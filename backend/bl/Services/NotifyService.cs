using BackendCommon.Interfaces;
using EasyNetQ;
using ProjCommon.Configurators;
using ProjCommon.DTO;
using ProjCommon.Enums;

namespace BackendBl.Services;

public class NotifyService : INotifyService
{
    private readonly IBus _queue;
    public NotifyService(IBus queue)
    {
        _queue = queue;
    }

    public async Task NotifyOrderStatusChanged(int orderId, Guid userId, OrderStatus newOrderStatus)
    {
        await _queue.SendReceive.SendAsync<StatusChangedNotification>(
            NotificationQueueConfigurator.DefaultQueueName,
            new StatusChangedNotification {
                OrderId = orderId, 
                Status = newOrderStatus,
                UserId = userId
            }
        );
    }
}