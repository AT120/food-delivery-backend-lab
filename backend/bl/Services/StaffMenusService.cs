using BackendCommon.DTO;
using BackendCommon.Interfaces;
using BackendDAL;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Exceptions;

namespace BackendBl.Services;

public class StaffMenusService : IStaffMenuService
{
    private readonly BackendDBContext _dbcontext;
    public StaffMenusService(BackendDBContext dbc)
    {
        _dbcontext = dbc;
    }


    private async Task<Guid> GetManagerRestaurant(Guid userId)
    {
        var manager = await _dbcontext.Managers.FindAsync(userId)
            ?? throw new BackendException(404, "User is not registered as a manager");
        return manager.RestaurantId;
    }


    private async Task<Menu> GetMenu(Guid userId, int menuId)
    {
        Guid restaurantId = await GetManagerRestaurant(userId);
        return await GetMenuFromRestaurant(restaurantId, menuId);
    }


    private async Task<Menu> GetMenuFromRestaurant(Guid restaurantId, int menuId)
    {
        var menu = await _dbcontext.Menus.FindAsync(menuId);

        if (menu == null || menu.RestaurantId != restaurantId)
            throw new BackendException(404, "Menu not found");

        return menu;
    }


    private async Task<bool> CheckMenuExistence(Guid restaurantId, int menuId)
    {
        return await _dbcontext.Menus
            .AnyAsync(m => m.Id == menuId && m.RestaurantId == restaurantId);
    }


    private async Task<bool> CheckDishExistence(Guid restaurantId, Guid dishId)
    {
        return await _dbcontext.Dishes
            .AnyAsync(m => m.Id == dishId && m.RestaurantId == restaurantId);
    }


    public async Task<Page<MenuShort>> GetMenus(int page, bool? archived, Guid managerId)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");

        Guid restauranId = await GetManagerRestaurant(managerId);
        var query = _dbcontext.Menus
            .Where(m => m.RestaurantId == restauranId);
        if (archived != null)
            query = query.Where(m => m.Archivied == archived);

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
        var restaurantId = await GetManagerRestaurant(managerId);
        var menu = new Menu
        {
            Name = name,
            RestaurantId = restaurantId,
        };
        if (dishes != null)
        {
            foreach (var dishId in dishes)
            {
                if (await CheckDishExistence(restaurantId, dishId))
                    menu.Dishes.Add(new Dish { Id = dishId });
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
            menu.Archivied = archived.Value;

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
        Guid restaurantId = await GetManagerRestaurant(managerId);
        var menu = await _dbcontext.Menus
            .Include(m => m.Dishes)
            .FirstOrDefaultAsync(m => m.Id == menuId);

        if (menu == null || menu.RestaurantId != restaurantId)
            throw new BackendException(404, "Menu not found");

        var dishes = menu.Dishes.Select(d => new DishShort //TODO: а сработает ли? Лучше переделать на явный include
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
            Name = menu.Name
        };
    }


    public async Task AddDishToMenu(int menuId, Guid dishId, Guid managerId)
    {
        Guid restaurantId = await GetManagerRestaurant(managerId);
        var menu = await GetMenuFromRestaurant(restaurantId, menuId);
        // if (!await CheckMenuExistence(restaurantId, menuId))
        // throw new BackendException(404, "Menu not found");

        if (!await CheckDishExistence(restaurantId, dishId))
            throw new BackendException(404, "Dish not found");

        menu.Dishes.Add(new Dish { Id = dishId });

        await _dbcontext.SaveChangesAsync();
    }


    public async Task DeleteDishFromMenu(int menuId, Guid dishId, Guid managerId)
    {
        // var restauranId = await GetManagerRestaurant(managerId);
        var menu = await GetMenu(managerId, menuId);
        menu.Dishes.Remove(new Dish { Id = dishId }); //TODO: Это может не работать
        await _dbcontext.SaveChangesAsync();
    }
}