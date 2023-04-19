using System.Security.Claims;
using System.Text;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace BackendBl.Services;

public class OrderService : IOrderService
{
    private readonly BackendDBContext _dbcontext;
    private readonly CartService _cartService;
    public OrderService(BackendDBContext dc, CartService cs)
    {
        _dbcontext = dc;
        _cartService = cs;
    }

    private static string GetArchivedDishesError(IEnumerable<DishInCart> dishes)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{dishes.Count()} dishes are not available");
        foreach (var dish in dishes)
        {
            sb.AppendLine($"{dish.DishId} {dish.Dish.Name}");
        }

        return sb.ToString();
    }

    public async Task<Page<CustomerOrderShortDTO>> GetOrders(
        ClaimsPrincipal user,
        int page,
        DateTime? startDate,
        DateTime? endDate,
        int status,
        int? orderIdQuery //TODO: Sorting
    )
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);

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
            query = query.Where(o => o.Id == orderIdQuery); //TODO: Есть ли в этом необходимость?


        int size = await query.CountAsync();
        int rangeStart = PageSize.Default * (page - 1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        var orders = await query
            .Select(o => Converter.GetCustomerOrderShort(o))
            .Skip(rangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<CustomerOrderShortDTO>
        {
            PageInfo = new PageInfo(rangeStart, rangeEnd, size),
            Items = orders
        };
    }


    public async Task<CustomerDetailedOrderDTO> GetOrder(int orderId, Guid userId)
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
        ClaimsPrincipal user,
        string address,
        DateTime deliveryTime
    )
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        var dishesToOrder = await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .Include(d => d.Dish)
            .ToListAsync();

        if (dishesToOrder.Count == 0)
            throw new BackendException(400, "Can't place the order, your cart is empty.");

        var archivedDishes = dishesToOrder.Where(d => d.Dish.Archived);
        if (archivedDishes.IsNullOrEmpty())
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

        foreach (var dish in dishesToOrder)
        {
            await _dbcontext.OrderedDishes.AddAsync(new OrderedDish
            {
                Count = dish.Count,
                DishId = dish.DishId,
                OrderId = order.Id,
            });
        }
        
        await _cartService.CleanCart(user);
        await _dbcontext.SaveChangesAsync();

        return order.Id;
    }


    public async Task RepeatOrder(int orderId, ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        
        var prevOrder = await _dbcontext.Orders
            .Where(o => o.Id == orderId && o.CustomerId == userId)
            .Include(o => o.Dishes)
            .ThenInclude(d => d.Dish)
            .FirstOrDefaultAsync();
        
        if (prevOrder is null)
            throw new BackendException(404, "User does not have orders with requested id");
        
        foreach (var dish in prevOrder.Dishes)
        {
            if (dish.Dish.Archived)
                continue;
            
            await _dbcontext.DishesInCart.AddAsync(new DishInCart
            {
                Count = dish.Count,
                CustomerId = userId,
                DishId = dish.DishId
            });
        }
    }


    public async Task CancelOrder(int orderId, ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        var order = await _dbcontext.Orders
            .Where(o => o.Id == orderId && o.CustomerId == userId)
            .FirstOrDefaultAsync();
        
        if (order is null)
            throw new BackendException(404, "User does not have orders with requested id");
        
        if (order.Status != OrderStatus.Created)
            throw new BackendException(400, "Your order is already being processed. You can't cancel it.")
        
        //TODO: race condition
        order.Status = OrderStatus.Canceled;
        return;
    }
}