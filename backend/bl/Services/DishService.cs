using System.Linq.Expressions;
using System.Reflection;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        int rangeStart = PageSize.Default * (page-1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        var dishes = await query
            .Select(d => new DishShort 
            {
                Id = d.Id,
                IsVegetarian = d.IsVegetarian,
                Name = d.Name,
                PhotoUrl = d.PhotoURL,
                Price = d.Price,
            })
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
}