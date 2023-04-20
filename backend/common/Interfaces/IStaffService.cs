using System.Security.Claims;
using BackendCommon.DTO;
using BackendCommon.Enums;

namespace BackendCommon.Interfaces;

public interface IStaffService
{
    Task CancelOrder(int orderId, Guid userId);

    Task<Page<StaffOrderDTO>> GetOrders(
        int page,
        int orderStatuses,
        int? orderId,
        StaffOrderSortingTypes sorting,
        ClaimsPrincipal user
    );
    
    Task<Page<CourierOrderDTO>> GetCourierOrders(
        int page,
        bool inDelivery,
        StaffOrderSortingTypes sorting,
        Guid userId
    );
    
    Task NextStatus(int orderId, ClaimsPrincipal user);
}