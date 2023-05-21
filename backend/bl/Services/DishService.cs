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
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

namespace BackendBl.Services;

public class DishService : IDishService
{
    private readonly BackendDBContext _dbcontext;
    public DishService(BackendDBContext dc)
    {
        _dbcontext = dc;
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
            query = query.Where(d => d.Menus.Any(menu => menus!.Contains(menu.Id)));

        if (!categories.IsNullOrEmpty())
            query = query.Where(d => categories!.Contains(d.Category));
        

        query = SortingHelper.DishSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        PageInfo pageInfo = new (page, size, PageSize.Default);

        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Invalid page number");

        var dishes = await query
            .Select(d => Converter.GetShortDish(d))
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<DishShort>
        {
            PageInfo = pageInfo,
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