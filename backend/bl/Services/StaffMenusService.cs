using BackendCommon.DTO;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Exceptions;

namespace BackendBl.Services;

public class StaffMenusService : IStaffMenuService
{
    private readonly BackendDBContext _dbcontext;
    private readonly IStaffHelperService _staffHelper;
    public StaffMenusService(BackendDBContext dbc, IStaffHelperService sh)
    {
        _dbcontext = dbc;
        _staffHelper = sh;
    }



    private async Task<Menu> GetMenu(Guid userId, int menuId)
    {
        Guid restaurantId = await _staffHelper.GetManagerRestaurant(userId);
        return await GetMenuFromRestaurant(restaurantId, menuId);
    }


    private async Task<Menu> GetMenuFromRestaurant(Guid restaurantId, int menuId)
    {
        var menu = await _dbcontext.Menus.FindAsync(menuId);

        if (menu == null || menu.RestaurantId != restaurantId)
            throw new BackendException(404, "Menu not found");

        return menu;
    }


    public async Task<Page<MenuShort>> GetMenus(int page, bool? archived, Guid managerId)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");

        Guid restauranId = await _staffHelper.GetManagerRestaurant(managerId);
        var query = _dbcontext.Menus
            .Where(m => m.RestaurantId == restauranId);
        if (archived != null)
            query = query.Where(m => m.Archived == archived);

        int size = await query.CountAsync();
        PageInfo pageInfo = new(page, size, PageSize.Default);
        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Invalid page number");

        var menus = await query
            .Select(m => new MenuShort
            {
                Id = m.Id,
                Name = m.Name
            }).OrderBy(m => m.Id)
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.Default)
            .ToListAsync();

        return new Page<MenuShort>
        {
            PageInfo = pageInfo,
            Items = menus
        };
    }


    public async Task<int> CreateMenu(string name, IEnumerable<Guid>? dishes, Guid managerId)
    {
        var restaurantId = await _staffHelper.GetManagerRestaurant(managerId);
        var menu = new Menu
        {

            Name = name,
            RestaurantId = restaurantId,
        };
        if (dishes != null)
        {
            foreach (var dishId in dishes)
            {
                var dish = await _dbcontext.Dishes.FindAsync(dishId);
                if (dish != null && dish.RestaurantId == restaurantId)
                    menu.Dishes.Add(dish);     
            }
        }
        await _dbcontext.Menus.AddAsync(menu);
        await _dbcontext.SaveChangesAsync();
        return menu.Id;
    }


    public async Task EditMenu(int menuId, bool? archived, string? name, Guid managerId)
    {
        var menu = await GetMenu(managerId, menuId);
        if (archived != null)
            menu.Archived = archived.Value;

        if (name != null)
            menu.Name = name;

        await _dbcontext.SaveChangesAsync();
    }


    public async Task DeleteMenu(int menuId, Guid managerId)
    {
        var menu = await GetMenu(managerId, menuId);
        _dbcontext.Menus.Remove(menu);
        await _dbcontext.SaveChangesAsync();
    }


    public async Task<MenuDetailed> GetMenuDetailed(int menuId, Guid managerId)
    {
        Guid restaurantId = await _staffHelper.GetManagerRestaurant(managerId);
        var menu = await _dbcontext.Menus
            .Include(m => m.Dishes)
            .FirstOrDefaultAsync(m => m.Id == menuId);

        if (menu == null || menu.RestaurantId != restaurantId)
            throw new BackendException(404, "Menu not found");

        var dishes = menu.Dishes.Select(d => new DishShort
        {
            Id = d.Id,
            IsVegetarian = d.IsVegetarian,
            Name = d.Name,
            PhotoUrl = d.PhotoURL,
            Price = d.Price
        });

        return new MenuDetailed
        {
            Dishes = dishes,
            Id = menu.Id,
            Name = menu.Name,
            Archived = menu.Archived
        };
    }


    public async Task AddDishToMenu(int menuId, Guid dishId, Guid managerId)
    {
        Guid restaurantId = await _staffHelper.GetManagerRestaurant(managerId);
        var menu = await GetMenuFromRestaurant(restaurantId, menuId);
        // if (!await CheckMenuExistence(restaurantId, menuId))
        // throw new BackendException(404, "Menu not found");
        var dish = await _dbcontext.Dishes.FindAsync(dishId);
        if (dish == null || dish.RestaurantId != restaurantId)
            throw new BackendException(404, "Dish not found");

        menu.Dishes.Add(dish);

        await _dbcontext.SaveChangesAsync();
    }


    public async Task DeleteDishFromMenu(int menuId, Guid dishId, Guid managerId)
    {
        var restauranId = await _staffHelper.GetManagerRestaurant(managerId);
        bool menuExists = await _staffHelper.CheckMenuExistence(restauranId, menuId);
        if (!menuExists)
            throw new BackendException(404, "Menu not found");

        await _dbcontext.DishesInMenu
            .Where(d => d.DishId == dishId && d.MenuId == menuId)
            .ExecuteDeleteAsync();
    }
}