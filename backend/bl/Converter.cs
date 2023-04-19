using BackendCommon.DTO;
using BackendDAL.Entities;
using Microsoft.AspNetCore.Diagnostics;

namespace BackendBl;

public static class Converter
{
    public static DishShort GetShortDish(Dish d)
    {
        return new DishShort
        {
            Id = d.Id,
            IsVegetarian = d.IsVegetarian,
            Name = d.Name,
            PhotoUrl = d.PhotoURL,
            Price = d.Price
        };
    }

    public static DishInCartDTO GetDishInCartDTO(DishInCart cart)
    {
        return new DishInCartDTO
        {
            Count = cart.Count,
            Dish = GetShortDish(cart.Dish),
        };
    }

    public static CustomerOrderShortDTO GetCustomerOrderShort(Order order)
    {
        return new CustomerOrderShortDTO
        {
            DeliveryTime = order.DeliveryTime,
            Id = order.Id,
            OrderTime = order.OrderTime,
            Price = order.FinalPrice,
            Status = order.Status
        };
    }


    public static CustomerDetailedOrderDTO GetCustomerDetailedOrder(Order order)
    {
        return new CustomerDetailedOrderDTO
        {
            Id = order.Id,
            Address = order.Address,
            DeliveryTime = order.DeliveryTime,
            OrderTime = order.OrderTime,
            FinalPrice = order.FinalPrice,
            RestaurantId = order.RestaurantId,
            Status = order.Status,

            Dishes = order.Dishes.Select(d => GetShortDish(d.Dish)).ToList()
        };
    }

}