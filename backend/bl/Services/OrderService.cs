using System.Text;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Exceptions;

namespace BackendBl.Services;

public class OrderService : IOrderService
{
    private readonly BackendDBContext _dbcontext;
    private readonly ICartService _cartService;
    public OrderService(BackendDBContext dc, ICartService cs)
    {
        _dbcontext = dc;
        _cartService = cs;
    }

    private static string GetArchivedDishesError(IEnumerable<BackendDAL.Entities.DishInCart> dishes)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{dishes.Count()} dishes are not available");
        foreach (var dish in dishes)
        {
            sb.AppendLine($"{dish.DishId} {dish.Dish.Name}");
        }

        return sb.ToString();
    }

    public async Task<Page<CustomerOrderShort>> GetOrders(
        Guid userId,
        int page,
        DateTime? startDate,
        DateTime? endDate,
        int status,
        int? orderIdQuery
    )
    {
        var query = _dbcontext.Orders
            .Where(o =>
                ((int)o.Status & status) != 0 &&
                o.CustomerId == userId
            );

        if (startDate is not null)
            query = query.Where(o => o.OrderTime >= startDate);
        if (endDate is not null)
            query = query.Where(o => o.OrderTime <= endDate);
        if (orderIdQuery is not null)
            query = query.Where(o => o.Id.ToString().Contains(orderIdQuery.ToString()));


        int size = await query.CountAsync();
        PageInfo pageInfo = new (page, size, PageSize.Default);

        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Invalid page number");

        var orders = await query
            .Select(order =>
            new CustomerOrderShort
            {
                DeliveryTime = order.DeliveryTime,
                Id = order.Id,
                OrderTime = order.OrderTime,
                Price = order.FinalPrice,
                Status = order.Status
            })
            .OrderBy(o => o.OrderTime)
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<CustomerOrderShort>
        {
            PageInfo = pageInfo,
            Items = orders
        };
    }


    public async Task<CustomerDetailedOrder> GetOrder(int orderId, Guid userId)
    {
        Order order = await _dbcontext.Orders
            .Include(o => o.Dishes)
            .ThenInclude(d => d.Dish)
            .FirstOrDefaultAsync(o =>
                o.Id == orderId &&
                o.CustomerId == userId
            ) ?? throw new BackendException(404, "This customer has no order with specified id");

        return Converter.GetCustomerDetailedOrder(order);
    }


    public async Task<int> CreateOrderFromCart(
        Guid userId,
        string address,
        DateTime deliveryTime
    )
    {
        if (deliveryTime - DateTime.UtcNow < Delivery.MinDeliveryTimeDiff)
            throw new BackendException(400, "We can't delivery order faster the 2 hours");

        var dishesToOrder = await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .Include(d => d.Dish)
            .ToListAsync();

        if (dishesToOrder.Count == 0)
            throw new BackendException(400, "Can't place the order, your cart is empty.");

        var archivedDishes = dishesToOrder.Where(d => d.Dish.Archived);
        if (!archivedDishes.IsNullOrEmpty())
            throw new BackendException(400, GetArchivedDishesError(archivedDishes));

        // Из одного ресторана
        var restaurantId = dishesToOrder.First().Dish.RestaurantId;
        if (dishesToOrder.Skip(1).Any(d => d.Dish.RestaurantId != restaurantId))
            throw new BackendException(
                400,
                "You have dishes from differrent restaurants in your cart. It is not allowed."
            );


        int price = dishesToOrder.Sum(d => d.Count * d.Dish.Price);
        var order = new Order
        {
            CustomerId = userId,
            Address = address,
            OrderTime = DateTime.UtcNow,
            DeliveryTime = deliveryTime,
            FinalPrice = price,
            RestaurantId = restaurantId,
            Status = OrderStatus.Created
        };

        await _dbcontext.Orders.AddAsync(order); //TODO: будет ли здесь id
        await _dbcontext.SaveChangesAsync();

        foreach (var dish in dishesToOrder)
        {
            await _dbcontext.OrderedDishes.AddAsync(new OrderedDish
            {
                Count = dish.Count,
                DishId = dish.DishId,
                OrderId = order.Id,
            });
        }

        await _cartService.CleanCart(userId);
        await _dbcontext.SaveChangesAsync();

        return order.Id;
    }


    public async Task RepeatOrder(int orderId, Guid userId)
    {
        var prevOrder = await _dbcontext.Orders
            .Where(o => o.Id == orderId && o.CustomerId == userId)
            .Include(o => o.Dishes)
            .ThenInclude(d => d.Dish)
            .FirstOrDefaultAsync();

        if (prevOrder is null)
            throw new BackendException(404, "User does not have orders with requested id");
        var cart = await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .ToListAsync();

        foreach (var dish in prevOrder.Dishes)
        {
            if (dish.Dish.Archived)
                continue;

            var sameDishInCart = cart.FirstOrDefault(d => d.DishId == dish.DishId);

            if (sameDishInCart is not null)
            {
                sameDishInCart.Count = dish.Count;
            }
            else
            {
                await _dbcontext.DishesInCart.AddAsync(new BackendDAL.Entities.DishInCart
                {
                    Count = dish.Count,
                    CustomerId = userId,
                    DishId = dish.DishId
                });
            }
        }

        await _dbcontext.SaveChangesAsync();
    }


    public async Task CancelOrder(int orderId, Guid userId)
    {
        var order = await _dbcontext.Orders
            .Where(o => o.Id == orderId && o.CustomerId == userId)
            .FirstOrDefaultAsync();

        if (order is null)
            throw new BackendException(404, "User does not have orders with requested id");

        if (order.Status != OrderStatus.Created)
            throw new BackendException(400, "Your order is already being processed. You can't cancel it.");

        //TODO: race condition
        order.Status = OrderStatus.Canceled;

        await _dbcontext.SaveChangesAsync();
        return;
    }
}