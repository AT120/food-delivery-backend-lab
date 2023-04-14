using BackendCommon.DTO;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore.Design.Internal;

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
}