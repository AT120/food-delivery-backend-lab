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

    private static Expression<Func<TPropertyBase, bool>> 
        GetOrExpression<T, TPropertyBase>(IEnumerable<T> possibleOptions, PropertyInfo property)
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


    public async Task<Page<DishShort>> GetDishes(
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

        query = SortingHelper.DishSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        int rangeStart = PageSize.Default * (page - 1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        if (rangeStart > size)
            throw new BackendException(400, "Invalid page number");

        var dishes = await query
            .Select(d => Converter.GetShortDish(d))
            .Skip(rangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<DishShort>
        {
            PageInfo = new PageInfo(rangeStart, rangeEnd, size),
            Items = dishes
        };
    }


    public async Task<DishDetailed> GetDish(Guid dishId, Guid? userId)
    {
        var dish = await _dbcontext.Dishes.FindAsync(dishId)
            ?? throw new BackendException(404, "Requested dish does not exist.");

        RatedDish? rating = null;
        if (userId is not null)
            rating = await _dbcontext.RatedDishes.FindAsync(userId, dishId);

        bool userCanRate = rating is not null;
        int? previousRating = rating?.Rating;

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
        int newRating,
        Guid userId)
    {
        //TODO: mutex
        var dish = await _dbcontext.Dishes.FindAsync(dishId)
            ?? throw new BackendException(404, "Requested dish does not exist.");

        var ratedDish = await _dbcontext.RatedDishes.FindAsync(userId, dishId)
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