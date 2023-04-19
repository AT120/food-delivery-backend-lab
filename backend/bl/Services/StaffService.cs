using System.Runtime.CompilerServices;
using System.Security.Claims;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace BackendBl.Services;


public class StaffService
{
    private readonly BackendDBContext _dbcontext;
    public StaffService(BackendDBContext dc)
    {
        _dbcontext = dc;
    }

    private readonly BackendException UnauthorizedChangeStatus
        = new(403, "You have no rights to change the status of this order.");

    private readonly BackendException Unregistered
        = new(403, "You are not registered in the sytem.");

    private static OrderStatus GetNextOrderStatus(OrderStatus status)
    {
        if (((int)status) >= ((int)OrderStatus.Delivered))
            throw new ArgumentException("Passed status can not be incremented");

        return (OrderStatus)((int)status << 1);
    }


    private async Task NextStatusCook(Order order, Guid userId)
    {
        var cook = await _dbcontext.Cooks.FindAsync(userId);
        Guid? cookRestId = cook?.RestaurantId;

        if (cookRestId is null || cookRestId != order.RestaurantId)
            throw UnauthorizedChangeStatus;  // заказ из одного ресторана, а повар из другого

        if ((int)order.Status >= (int)OrderStatus.AwaitsCourier)
            throw UnauthorizedChangeStatus;  // заказ отдан курьеру или отменен

        if (order.CookId != userId)
            throw UnauthorizedChangeStatus;  // другой повар уже работает над этим заказом

        order.CookId ??= userId;  // TODO: Тут может быть дефолт, и ничего не сработает
        order.Status = GetNextOrderStatus(order.Status);

        await _dbcontext.SaveChangesAsync();
    }

    private async Task NextStatusCourier(Order order, Guid userId)
    {
        if (!await _dbcontext.Couriers.AnyAsync(x => x.Id == userId))
            throw UnauthorizedChangeStatus;

        if (order.Status != OrderStatus.AwaitsCourier && order.Status != OrderStatus.Delivery)
            throw UnauthorizedChangeStatus;

        if (order.CourierId != userId)
            throw UnauthorizedChangeStatus;

        order.CourierId ??= userId;
        order.Status = GetNextOrderStatus(order.Status);

        await _dbcontext.SaveChangesAsync();
    }


    public async Task NextStatus(int orderId, ClaimsIdentity user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        var userRoles = user.FindAll(x => x.Type == ClaimType.Role);
        var isCook = userRoles.Any(x => x.Value == RoleType.Cook.ToString());
        var isCourier = userRoles.Any(x => x.Value == RoleType.Courier.ToString());

        var order = await _dbcontext.Orders.FindAsync(orderId)
            ?? throw new BackendException(404, "There is no order with specified id.");

        if (isCook)
            await NextStatusCook(order, userId);
        else if (isCourier)
            await NextStatusCourier(order, userId);
        else
            throw UnauthorizedChangeStatus;

        //TODO: Race condition? 
    }

    public async Task<Page<CourierOrderDTO>> GetCourierOrders(
        int page,
        bool inDelivery,
        StaffOrderSortingTypes sorting,
        Guid userId)
    {

        if (page < 1)
            throw new BackendException(400, "Invalid page number");

        bool registered = _dbcontext.Couriers.Any(c => c.Id == userId);
        if (!registered)
            throw Unregistered;

        var query = _dbcontext.Orders.AsQueryable();
        if (inDelivery)
        {
            query = query.Where(o =>
                o.Status == OrderStatus.Delivery &&
                o.CourierId == userId);

        }
        else
        {
            query = query.Where(o => o.Status == OrderStatus.AwaitsCourier);
        }

        query = SortingHelper.StaffSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        int rangeStart = PageSize.Default * (page - 1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        var orders = await query
            .Select(o => new CourierOrderDTO
            {
                Address = o.Address,
                Id = o.Id,
                RestaurantId = o.RestaurantId,
            })
            .Skip(rangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<CourierOrderDTO>
        {
            PageInfo = new PageInfo(rangeStart, rangeEnd, size),
            Items = orders
        };
    }


    private async Task<Page<StaffOrderDTO>> GetOrdersCook(
        int page,
        int orderStatuses,
        int? orderId, //TODO: Сделать поиск по id
        StaffOrderSortingTypes sorting,
        Guid userId)
    {
        bool createdStatus = (orderStatuses & (int)OrderStatus.Created) != 0;
        var cook = await _dbcontext.Cooks.FindAsync(userId)
            ?? throw Unregistered;

        var query = _dbcontext.Orders
            .Include(o => o.Dishes)
            .ThenInclude(d => d.Dish)
            .Where(o =>
                o.RestaurantId == cook.RestaurantId &&
                (
                    (((int)o.Status & orderStatuses) != 0 && o.CookId == userId) ||     // заказы сделанные поваром
                    (createdStatus && (o.Status == OrderStatus.Created))                // заказы доступные для повара
                )
            );

        query = SortingHelper.StaffSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        int rangeStart = PageSize.Default * (page - 1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        var orders = await query
            .Select(o => new StaffOrderDTO
            {
                Id = o.Id,
                Status = o.Status,
                OrderTime = o.OrderTime,
                DeliveryTime = o.DeliveryTime,
                Dishes = o.Dishes.Select(d => Converter.GetShortDish(d.Dish)).ToList()
            })
            .Skip(rangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<StaffOrderDTO>
        {
            PageInfo = new PageInfo(rangeStart, rangeEnd, size),
            Items = orders
        };
    }

    private async Task<Page<StaffOrderDTO>> GetOrdersManager(
        int page,
        int orderStatuses,
        int? orderId, //TODO: Сделать поиск по id
        StaffOrderSortingTypes sorting,
        Guid userId)
    {
        var manager = await _dbcontext.Managers.FindAsync(userId)
            ?? throw Unregistered;

        var query = _dbcontext.Orders
            .Include(o => o.Dishes)
            .ThenInclude(d => d.Dish)
            .Where(o =>
                o.RestaurantId == manager.RestaurantId &&
                ((int)o.Status & orderStatuses) != 0
            );

        query = SortingHelper.StaffSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        int rangeStart = PageSize.Default * (page - 1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        var orders = await query
            .Select(o => new StaffOrderDTO
            {
                Id = o.Id,
                CookId = o.CookId,
                CourierId = o.CourierId,
                CustomerId = o.CustomerId,
                Status = o.Status,
                OrderTime = o.OrderTime,
                DeliveryTime = o.DeliveryTime,
                Dishes = o.Dishes.Select(d => Converter.GetShortDish(d.Dish)).ToList()
            })
            .Skip(rangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<StaffOrderDTO>
        {
            PageInfo = new PageInfo(rangeStart, rangeEnd, size),
            Items = orders
        };
    }

    public async Task<Page<StaffOrderDTO>> GetOrders(
        int page,
        int orderStatuses,
        int? orderId, //TODO: Сделать поиск по id
        StaffOrderSortingTypes sorting,
        ClaimsPrincipal user)
    {
        if (page < 1)
            throw new BackendException(400, "Invalid page number");
        Guid userId = ClaimsHelper.GetUserId(user);
        var userRoles = user.FindAll(x => x.Type == ClaimType.Role);
        var isCook = userRoles.Any(x => x.Value == RoleType.Cook.ToString());
        var isManager = userRoles.Any(x => x.Value == RoleType.Manager.ToString());

        if (isCook)
            return await GetOrdersCook(page, orderStatuses, orderId, sorting, userId);
        else if (isManager)
            return await GetOrdersManager(page, orderStatuses, orderId, sorting, userId);
        else
            throw new BackendException(403, "Unathorized");
    }

    public async Task CancelOrder(int orderId, ClaimsIdentity user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        var isCourier = user.HasClaim(x =>
            x.Type == ClaimType.Role &&
            x.Value == RoleType.Courier.ToString()
        );

        var order = await _dbcontext.Orders.FindAsync(orderId)
            ?? throw new BackendException(404, "There is no order with specified id.");

        if (!isCourier)
            throw UnauthorizedChangeStatus;

        if (!await _dbcontext.Couriers.AnyAsync(x => x.Id == userId))
            throw UnauthorizedChangeStatus;

        // это еще дополнительно гарантирует, что заказ находится или находился в доставке
        if (order.CourierId != userId)
            throw UnauthorizedChangeStatus;

        if (order.Status != OrderStatus.Canceled)
            throw UnauthorizedChangeStatus;


        //TODO: race condition
        order.Status = OrderStatus.Canceled;
    }


}