using System.Net.Http.Headers;
using System.Security.Claims;
using BackendCommon.DTO;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;
using DishInCart = BackendDAL.Entities.DishInCart;

namespace BackendBl.Services;

public class CartService : ICartService
{
    private readonly BackendDBContext _dbcontext;
    public CartService(BackendDBContext dc)
    {
        _dbcontext = dc;
    }

    public async Task<Cart> GetCart(Guid userId)
    {
        var dishes = await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .Include(d => d.Dish)
            .Select(d => Converter.GetDishInCartDTO(d))
            .ToListAsync();

        int total = 0;
        foreach (var dish in dishes)
            total += dish.Count * dish.Dish.Price;

        return new Cart
        {
            Dishes = dishes,
            FinalPrice = total,
        };
    }


    public async Task PutDishIntoCart(DishCount dishModel, Guid userId)
    {
        if (dishModel.Count < 0)
            throw new BackendException(400, "Dish count have to be greter then 0");
        var dish = await _dbcontext.Dishes.FindAsync(dishModel.Id)
            ?? throw new BackendException(404, "Requested dish does not exist.");
        if (dish.Archived)
            throw new BackendException(400, "This dish is archived and can not be added into the cart.");
        if (dishModel.Count == 0)
            return;


        await _dbcontext.DishesInCart.AddAsync(new DishInCart
        {
            Count = dishModel.Count,
            CustomerId = userId,
            Dish = dish
        });

        await _dbcontext.SaveChangesAsync();
    }


    public async Task ChangeDishQunatity(Guid dishId, int count, Guid userId)
    {
        DishInCart dishInCart = await _dbcontext.DishesInCart.FindAsync(userId, dishId)
            ?? throw new BackendException(404, "Requested dish is absent in user's cart.");
        if (count < 0)
            throw new BackendException(400, "Dish count have to be greter then 0");

        if (count == 0)
            _dbcontext.DishesInCart.Remove(dishInCart);
        else
            dishInCart.Count = count;

        await _dbcontext.SaveChangesAsync();
    }


    public async Task DeleteDishFromCart(Guid dishId, Guid userId)
    {
        await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId && d.DishId == dishId)
            .ExecuteDeleteAsync();
    }


    public async Task CleanCart(Guid userId)
    {
        await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .ExecuteDeleteAsync();
    }


}