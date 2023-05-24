using BackendCommon.DTO;
using BackendDAL.Entities;
using Microsoft.AspNetCore.Diagnostics;

namespace BackendBl;

public static class Converter
{
    public static DishShort GetShortDish(Dish d, int? dishPrice = null)
    {
        return new DishShort
        {
            Id = d.Id,
            IsVegetarian = d.IsVegetarian,
            Name = d.Name,
            PhotoUrl = d.PhotoURL,
            Price = dishPrice ?? d.Price
        };
    }

    public static BackendCommon.DTO.DishInCart GetDishInCartDTO(BackendDAL.Entities.DishInCart cart)
    {
        return new BackendCommon.DTO.DishInCart
        {
            Count = cart.Count,
            Dish = GetShortDish(cart.Dish),
        };
    }

    public static CustomerOrderShort GetCustomerOrderShort(Order order)
    {
        return new CustomerOrderShort
        {
            DeliveryTime = order.DeliveryTime,
            Id = order.Id,
            OrderTime = order.OrderTime,
            Price = order.FinalPrice,
            Status = order.Status
        };
    }


    public static CustomerDetailedOrder GetCustomerDetailedOrder(Order order)
    {
        return new CustomerDetailedOrder
        {
            Id = order.Id,
            Address = order.Address,
            DeliveryTime = order.DeliveryTime,
            OrderTime = order.OrderTime,
            FinalPrice = order.FinalPrice,
            RestaurantId = order.RestaurantId,
            Status = order.Status,
 
            Dishes = order.Dishes.Select(d => GetShortDish(d.Dish, d.DishPrice)).ToList()
        };
    }

}