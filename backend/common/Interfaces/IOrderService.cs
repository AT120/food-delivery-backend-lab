using BackendCommon.DTO;
using ProjCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IOrderService
{
    Task<Page<CustomerOrderShort>> GetOrders(
        Guid userId,
        int page,
        DateTime? startDate,
        DateTime? endDate,
        int status,
        int? orderIdQuery
    );

    Task<CustomerDetailedOrder> GetOrder(int orderId, Guid userId);

    Task<int> CreateOrderFromCart(
        Guid userId,
        string address,
        DateTime deliveryTime
    );

    Task RepeatOrder(int orderId, Guid userId);
    Task CancelOrder(int orderId, Guid userId);
}