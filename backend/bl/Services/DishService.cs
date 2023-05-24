using System.Linq.Expressions;
using System.Reflection;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

namespace BackendBl.Services;

public class DishService : IDishService
{
    private readonly BackendDBContext _dbcontext;
    private readonly IStaffHelperService _staffHelper;
    public DishService(BackendDBContext dc, IStaffHelperService sh)
    {
        _dbcontext = dc;
        _staffHelper = sh;
    }


    private async Task<Dish> GetDish(Guid dishId, Guid managerId)
    {
        Guid restId = await _staffHelper.GetManagerRestaurant(managerId);
        Dish? dish = await _dbcontext.Dishes.FindAsync(dishId);
        if (dish == null || dish.RestaurantId != restId)
            throw new BackendException(404, "Dish not found");

        return dish;
    }


    private async Task<Page<DishShort>> GetDishes(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting,
        bool? archived = false)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");

        var restExists = await _dbcontext.Restaurants.AnyAsync(x => x.Id == restaurantId);
        if (!restExists)
            throw new BackendException(404, "Requested restaurant does not exist.");

        var query = _dbcontext.Dishes
            .Where(d => d.RestaurantId == restaurantId);

        if (archived != null)
            query = query.Where(d => d.Archived == archived);

        if (vegetarianOnly)
            query = query.Where(d => d.IsVegetarian);

        if (!menus.IsNullOrEmpty())
            query = query.Where(d => d.Menus.Any(menu => menus!.Contains(menu.Id)));

        if (!categories.IsNullOrEmpty())
            query = query.Where(d => categories!.Contains(d.Category));


        query = SortingHelper.DishSortingFuncs[sorting](query);

        int size = await query.CountAsync();
        PageInfo pageInfo = new(page, size, PageSize.Default);

        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Invalid page number");

        var dishes = await query
            .Select(d => Converter.GetShortDish(d, null))
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

    public async Task<Page<DishShort>> GetDishesCustomer(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting)
    {
        return await GetDishes(
            restaurantId,
            page,
            vegetarianOnly,
            menus,
            categories,
            sorting,
            false
        );
    }

    public async Task<Page<DishShort>> GetDishesManager(
        Guid managerId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting,
        bool? archived)
    {
        var restId = await _staffHelper.GetManagerRestaurant(managerId);
        return await GetDishes(
            restId,
            page,
            vegetarianOnly,
            menus,
            categories,
            sorting,
            archived
        );
    }
    

    public async Task RateDish(
        Guid dishId,
        int newRating,
        Guid userId)
    {
        //TODO: race condition
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
        dish.Rating += (newRating / newPeopleRated) - (prevRating / oldPeopleRated);
        dish.PeopleRated = (int)newPeopleRated;

        ratedDish.Rating = newRating;

        await _dbcontext.SaveChangesAsync();
    }

    public async Task EditDish(Guid dishId, Guid managerId, DishEdit newDish)
    {
        var dish = await GetDish(dishId, managerId);

        if (newDish.Category != null)
            dish.Category = newDish.Category.Value;

        if (newDish.Price != null)
            dish.Price = newDish.Price.Value;

        if (newDish.Description != null)
            dish.Description = newDish.Description;

        if (newDish.IsVegetarian != null)
            dish.IsVegetarian = newDish.IsVegetarian.Value;

        if (newDish.PhotoURL != null)
            dish.PhotoURL = newDish.PhotoURL;

        if (newDish.Archived != null)
            dish.Archived = newDish.Archived.Value;

        await _dbcontext.SaveChangesAsync();
    }


    public async Task<Guid> CreateDish(Guid managerId, DishCreate dish)
    {
        Guid restId = await _staffHelper.GetManagerRestaurant(managerId);
        
        var newDish = new Dish
        {
            Id = Guid.NewGuid(),
            Name = dish.Name,
            Description = dish.Description,
            Price = dish.Price,
            IsVegetarian = dish.IsVegetarian,
            PhotoURL = dish.PhotoURL,
            Category = dish.Category,
            PeopleRated = 0,
            RestaurantId = restId,
            // Menus = dish.MenuIds.Select(id => new Menu { Id = id }).ToList()
        };

        foreach (int menuId in dish.MenuIds)
        {
            if (await _staffHelper.CheckMenuExistence(restId, menuId))
                newDish.Menus.Add(new Menu {Id = menuId});
        }

        _dbcontext.Dishes.Add(newDish);
        await _dbcontext.SaveChangesAsync();
        return newDish.Id;
    }

}