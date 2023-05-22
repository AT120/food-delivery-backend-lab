using System.Security.Claims;
using BackendCommon.DTO;
using BackendCommon.Enums;
using ProjCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IStaffOrdersService
{
    Task CancelOrder(int orderId, Guid userId);

    Task<Page<StaffOrder>> GetOrders(
        int page,
        int orderStatuses,
        int? orderId,
        StaffOrderSortingTypes sorting,
        ClaimsPrincipal user
    );
    
    Task<Page<CourierOrder>> GetCourierOrders(
        int page,
        bool inDelivery,
        StaffOrderSortingTypes sorting,
        Guid userId
    );
    
    Task NextStatus(int orderId, ClaimsPrincipal user);
}