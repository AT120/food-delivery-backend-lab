using System.Security.Claims;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Enums;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

namespace BackendBl.Services;


public class StaffOrdersService: IStaffOrdersService
{
    private readonly BackendDBContext _dbcontext;
    private readonly INotifyService _notifyService;
    public StaffOrdersService(BackendDBContext dc, INotifyService ns)
    {
        _dbcontext = dc;
        _notifyService = ns;
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

    private async Task AddAbilityToRate(Guid customerId, int orderId)
    {
        var order = await _dbcontext.Orders
            .Include(o => o.Dishes)
            .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("There is no such order");

        foreach (var dish in order.Dishes)
        {
            bool alreadyCanRate = await _dbcontext.RatedDishes
                .AnyAsync(x => 
                    x.DishId == dish.DishId &&
                    x.CustomerId == customerId
                );

            if (alreadyCanRate)
                continue;

            await _dbcontext.RatedDishes.AddAsync(new RatedDish
            {
                DishId = dish.DishId,
                CustomerId = customerId,
                Rating = null
            });
        }

        await _dbcontext.SaveChangesAsync();
    }

    private async Task NextStatusCook(Order order, Guid userId)
    {
        var cook = await _dbcontext.Cooks.FindAsync(userId);
        Guid? cookRestId = cook?.RestaurantId;

        if (cookRestId is null || cookRestId != order.RestaurantId)
            throw UnauthorizedChangeStatus;  // заказ из одного ресторана, а повар из другого

        if ((int)order.Status >= (int)OrderStatus.AwaitsCourier)
            throw UnauthorizedChangeStatus;  // заказ отдан курьеру или отменен

        if (order.CookId is not null && order.CookId != userId)
            throw UnauthorizedChangeStatus;  // другой повар уже работает над этим заказом

        order.CookId ??= userId;
        order.Status = GetNextOrderStatus(order.Status);
        await _notifyService.NotifyOrderStatusChanged(order.Id, order.CustomerId, order.Status);

        await _dbcontext.SaveChangesAsync();
    }


    private async Task NextStatusCourier(Order order, Guid userId)
    {
        if (!await _dbcontext.Couriers.AnyAsync(x => x.Id == userId))
            throw UnauthorizedChangeStatus;

        if (order.Status != OrderStatus.AwaitsCourier && order.Status != OrderStatus.Delivery)
            throw UnauthorizedChangeStatus;

        if (order.CourierId is not null && order.CourierId != userId)
            throw UnauthorizedChangeStatus;

        order.CourierId ??= userId;
        order.Status = GetNextOrderStatus(order.Status);

        if (order.Status == OrderStatus.Delivered)
            await AddAbilityToRate(order.CustomerId, order.Id);

        await _dbcontext.SaveChangesAsync();
    }


    public async Task NextStatus(int orderId, ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetUserId(user);
        var userRoles = user.FindAll(x => x.Type == ClaimType.Role);
        var isCook = user.IsInRole(RoleType.Cook.ToString());
        var isCourier = user.IsInRole(RoleType.Courier.ToString());

        
        // try 
        // {
            var order = await _dbcontext.Orders.FindAsync(orderId)
                ?? throw new BackendException(404, "There is no order with specified id.");

            if (isCook)
                await NextStatusCook(order, userId);
            else if (isCourier)
                await NextStatusCourier(order, userId);
            else
                throw UnauthorizedChangeStatus;
        // }
        // finally
        // {

        // }
        //TODO: Race condition? 
    }

    public async Task<Page<CourierOrder>> GetCourierOrders(
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

        var query = _dbcontext.Orders.Where(o => o.RestaurantId != null);
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
        PageInfo pageInfo = new (page, size, PageSize.Default);

        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Invalid page number");     
        
        var orders = await query
            .Select(o => new CourierOrder
            {
                Address = o.Address,
                Id = o.Id,
                RestaurantId = o.RestaurantId.Value, // Проверенно на null выше
            })
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<CourierOrder>
        {
            PageInfo = pageInfo,
            Items = orders
        };
    }


    private async Task<Page<StaffOrder>> GetOrdersCook(
        int page,
        int orderStatuses,
        int? orderId,
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

        if (orderId is not null)
            query = query.Where(x => x.Id.ToString().Contains(orderId.ToString()));

        query = SortingHelper.StaffSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        PageInfo pageInfo = new (page, size, PageSize.Default);
        
        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Incorrect page number");     
        
        var orders = await query
            .Select(o => new StaffOrder
            {
                Id = o.Id,
                Status = o.Status,
                OrderTime = o.OrderTime,
                DeliveryTime = o.DeliveryTime,
                Dishes = o.Dishes.Select(d => Converter.GetShortDish(d.Dish, null)).ToList()
            })
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<StaffOrder>
        {
            PageInfo = pageInfo,
            Items = orders
        };
    }

    private async Task<Page<StaffOrder>> GetOrdersManager(
        int page,
        int orderStatuses,
        int? orderId,
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
        
        if (orderId is not null)
            query = query.Where(x => x.Id.ToString().Contains(orderId.ToString())); 

        query = SortingHelper.StaffSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        PageInfo pageInfo = new (page, size, PageSize.Default);
        
        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Incorrect page number");     
        
        var orders = await query
            .Select(o => new StaffOrder
            {
                Id = o.Id,
                CookId = o.CookId,
                CourierId = o.CourierId,
                CustomerId = o.CustomerId,
                Status = o.Status,
                OrderTime = o.OrderTime,
                DeliveryTime = o.DeliveryTime,
                Dishes = o.Dishes.Select(d => Converter.GetShortDish(d.Dish, null)).ToList()
            })
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<StaffOrder>
        {
            PageInfo = pageInfo,
            Items = orders
        };
    }

    public async Task<Page<StaffOrder>> GetOrders(
        int page,
        int orderStatuses,
        int? orderId, //TODO: Сделать поиск по id
        StaffOrderSortingTypes sorting,
        ClaimsPrincipal user)
    {
        if (page < 1)
            throw new BackendException(400, "Invalid page number");
        Guid userId = ClaimsHelper.GetUserId(user);
        var isCook = user.IsInRole(RoleType.Cook.ToString());
        var isManager = user.IsInRole(RoleType.Manager.ToString());

        if (isCook)
            return await GetOrdersCook(page, orderStatuses, orderId, sorting, userId);
        else if (isManager)
            return await GetOrdersManager(page, orderStatuses, orderId, sorting, userId);
        else
            throw new BackendException(403, "Unathorized");
    }

    public async Task CancelOrder(int orderId, Guid userId)
    {

        // var mutex = MutexHelper.OrderMutex(orderId);
        // // var semaphore = new Semaphore(1, 1, )
        // mutex.WaitOne();

        // try
        // {
            var order = await _dbcontext.Orders.FindAsync(orderId)
                ?? throw new BackendException(404, "There is no order with specified id.");


            if (!await _dbcontext.Couriers.AnyAsync(x => x.Id == userId))
                throw UnauthorizedChangeStatus;

            // это еще дополнительно гарантирует, что заказ находится или находился в доставке
            if (order.CourierId != userId)
                throw UnauthorizedChangeStatus;

            if (order.Status != OrderStatus.Canceled)
                throw UnauthorizedChangeStatus;

            //TODO: race condition
            order.Status = OrderStatus.Canceled;
            await _notifyService.NotifyOrderStatusChanged(order.Id, order.CustomerId, order.Status);
            await _dbcontext.SaveChangesAsync();
        // }
        // finally
        // {
        //     mutex.ReleaseMutex();
        // }
    }


}