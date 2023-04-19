using System.Security.Claims;
using BackendCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IOrderService
{
    Task<Page<CustomerOrderShortDTO>> GetOrders(
        ClaimsPrincipal user,
        int page,
        DateTime? startDate,
        DateTime? endDate,
        int status,
        int? orderIdQuery //TODO: Sorting
    );

    Task<CustomerDetailedOrderDTO> GetOrder(int orderId, Guid userId);

    Task<int> CreateOrderFromCart(
        ClaimsPrincipal user,
        string address,
        DateTime deliveryTime
    );

    Task RepeatOrder(int orderId, ClaimsPrincipal user);
    Task CancelOrder(int orderId, ClaimsPrincipal user);
}