using ProjCommon.Enums;

namespace BackendCommon.Interfaces;

public interface INotifyService
{
    Task NotifyOrderStatusChanged(int orderId, Guid userId, OrderStatus newOrderStatus);
}