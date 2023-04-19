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

namespace BackendBl.Services;

public class CartService : ICartService
{
    private readonly BackendDBContext _dbcontext;
    public CartService(BackendDBContext dc)
    {
        _dbcontext = dc;
    }

    public async Task<CartDTO> GetCart(ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        var dishes = await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .Include(d => d.Dish)
            .Select(d => Converter.GetDishInCartDTO(d))
            .ToListAsync();

        int total = 0;
        foreach (var dish in dishes)
            total += dish.Count * dish.Dish.Price;

        return new CartDTO
        {
            Dishes = dishes,
            FinalPrice = total,
        };
    }


    public async Task PutDishIntoCart(DishCount dishModel, ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        var dish = await _dbcontext.Dishes.FindAsync(dishModel.Id)
            ?? throw new BackendException(404, "Requested dish does not exist.");
        if (dish.Archived)
            throw new BackendException(400, "This dish is archived and can not be added into the cart.")


        await _dbcontext.DishesInCart.AddAsync(new DishInCart
        {
            Count = dishModel.Count,
            CustomerId = userId,
            Dish = dish
        });

        await _dbcontext.SaveChangesAsync();
    }


    public async Task ChangeDishQunatity(Guid dishId, ClaimsPrincipal user, int count)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        DishInCart dishInCart = await _dbcontext.DishesInCart.FindAsync(new { userId, dishId })
            ?? throw new BackendException(404, "Requested dish is absent in user's cart.");

        dishInCart.Count = count;
        await _dbcontext.SaveChangesAsync();
    }


    public async Task DeleteDishFromCart(Guid dishId, ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId && d.DishId == dishId)
            .ExecuteDeleteAsync();
    }


    public async Task CleanCart(ClaimsPrincipal user)
    {
        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
        await _dbcontext.DishesInCart
            .Where(d => d.CustomerId == userId)
            .ExecuteDeleteAsync();
    }


}