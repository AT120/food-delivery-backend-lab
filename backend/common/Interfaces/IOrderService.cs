using System.Security.Claims;
using BackendCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IOrderService
{
    Task<Page<CustomerOrderShortDTO>> GetOrders(
        Guid userId,
        int page,
        DateTime? startDate,
        DateTime? endDate,
        int status,
        int? orderIdQuery //TODO: Sorting
    );

    Task<CustomerDetailedOrderDTO> GetOrder(int orderId, Guid userId);

    Task<int> CreateOrderFromCart(
        Guid userId,
        string address,
        DateTime deliveryTime
    );

    Task RepeatOrder(int orderId, Guid userId);
    Task CancelOrder(int orderId, Guid userId);
}