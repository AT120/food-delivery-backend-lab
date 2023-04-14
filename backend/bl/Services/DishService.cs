using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace BackendBl.Services;

public class DishService : IDishService
{
    private readonly BackendDBContext _dbcontext;
    public DishService(BackendDBContext dc)
    {
        _dbcontext = dc;
    }

    public Expression<Func<TPropertyBase, bool>> GetOrExpression<T, TPropertyBase>(IEnumerable<T> possibleOptions, PropertyInfo property)
    {
        var arg = Expression.Parameter(typeof(TPropertyBase));

        var expr = Expression.Equal(
            Expression.Constant(possibleOptions.First()),
            Expression.MakeMemberAccess(arg, property)
        );

        foreach (var option in possibleOptions.Skip(1))
        {
            var disjunction = Expression.Equal(
                Expression.Constant(option),
                Expression.MakeMemberAccess(arg, property)
            );
            expr = Expression.OrElse(expr, disjunction);
        }

        return Expression.Lambda<Func<TPropertyBase, bool>>(expr, arg);
    }


    public async Task<DishesPage> GetDishes(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");

        var restExists = await _dbcontext.Restaurants.AnyAsync(x => x.Id == restaurantId);
        if (!restExists)
            throw new BackendException(404, "Requested restaurant does not exist.");

        var query = _dbcontext.Dishes
            .Where(d => d.RestaurantId == restaurantId)
            .Where(d => !d.Archived);

        if (vegetarianOnly)
            query = query.Where(d => d.IsVegetarian);

        if (!menus.IsNullOrEmpty())
        {
            var filter = GetOrExpression<int, Dish>(
                menus,
                typeof(Dish).GetProperty(nameof(Dish.MenuId))
            );

            query = query.Where(filter);
        }


        if (!categories.IsNullOrEmpty())
        {
            var filter = GetOrExpression<DishCategory, Dish>(
                categories,
                typeof(Dish).GetProperty(nameof(Dish.Category))
            );

            query = query.Where(filter);
        }

        query = SortingHelper.SortingFuncs[sorting](query);

        int size = await query.CountAsync();
        int rangeStart = PageSize.Default * (page - 1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        var dishes = await query
            .Select(d => Converter.GetShortDish(d))
            .Skip(rangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new DishesPage
        {
            Page = new PageInfo
            {
                RangeStart = (size == 0) ? 0 : rangeStart + 1,
                RangeEnd = rangeEnd,
                Size = size,
            },
            Dishes = dishes
        };
    }

    public async Task<DishDetailed> GetDish(Guid dishId, ClaimsPrincipal user)
    {
        var dish = await _dbcontext.Dishes.FindAsync(dishId)
            ?? throw new BackendException(404, "Requested dish does not exist.");

        bool userCanRate = false;
        int? previousRating = null;
        try
        {
            Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, user);
            var rating = await _dbcontext.RatedDishes
                .FindAsync(new { userId, dishId });

            userCanRate = rating is not null;
            previousRating = rating?.Rating;
        }
        catch { }

        return new DishDetailed
        {
            CanBeRated = userCanRate,
            Category = dish.Category,
            Description = dish.Description,
            Id = dish.Id,
            IsVegetarian = dish.IsVegetarian,
            Name = dish.Name,
            PhotoUrl = dish.PhotoURL,
            Price = dish.Price,
            Rating = dish.Rating,
            Archived = dish.Archived,
            PreviousRating = previousRating,
        };
    }


    public async Task RateDish(
        Guid dishId,
        ClaimsPrincipal userPrincipal,
        int newRating)
    {
        //TODO: mutex
        var dish = await _dbcontext.Dishes.FindAsync(dishId)
            ?? throw new BackendException(404, "Requested dish does not exist.");

        Guid userId = ClaimsHelper.GetValue<Guid>(ClaimType.UserId, userPrincipal);
        var ratedDish = await _dbcontext.RatedDishes.FindAsync(new { userId, dishId })
            ?? throw new BackendException(
                403,
                "You can not rate this dish",
                $"User {userId} tried to rate {dishId}"
            );
        double prevRating = ratedDish.Rating ?? 0;
        double newPeopleRated = (ratedDish.Rating is null) ? dish.PeopleRated + 1 : dish.PeopleRated;
        
        // чтобы избежать деления на 0, присвоим 1. ничего не сломается, т.к. в этом случае prevRating = 0.
        double oldPeopleRated = (dish.PeopleRated == 0) ? 1 : dish.PeopleRated;     
        dish.Rating += (newRating / newPeopleRated)  - (prevRating / oldPeopleRated);
        dish.PeopleRated = (int)newPeopleRated;
        
        ratedDish.Rating = newRating;

        await _dbcontext.SaveChangesAsync();
    }
}