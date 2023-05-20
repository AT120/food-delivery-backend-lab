using AdminCommon.DTO;
using AdminCommon.Interfaces;
using AuthDAL;
using AuthDAL.Models;
using BackendCommon.Enums;
using BackendDAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjCommon.DTO;
using ProjCommon.Enums;
using ProjCommon.Exceptions;
using ProjCommon.Interfaces;

namespace AdminBL.Services;

public class AdminRestaurantService : IAdminRestaurantService
{
    private readonly IRestaurantService _restaurantService;
    private readonly BackendDBContext _backendDBContext;
    private readonly AuthDBContext _authDBContext;
    private readonly UserManager<User> _userManager;

    public AdminRestaurantService(
        IRestaurantService rs,
        BackendDBContext bdb,
        UserManager<User> um,
        AuthDBContext adb)
    {
        _backendDBContext = bdb;
        _restaurantService = rs;
        _userManager = um;
        _authDBContext = adb;
    }


    private async Task<bool> AllOrdersAreCompleted(Guid restaurantId)
    {
        return await _backendDBContext.Orders
            .Where(o => o.RestaurantId == restaurantId)
            .AllAsync(
                o => ((int)o.Status & (int)OrderStatus.Completed) != 0
            );
    }

    private async Task RemoveFromRole(Guid userId, RoleType role)
    {
        User user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new ArgumentException("Пользователь не найден");
        
        var res = await _userManager.RemoveFromRoleAsync(user, role.ToString());
        if (!res.Succeeded)
            throw new BackendException(400, res.Errors.First().Description);

        if (role == RoleType.Cook)
            _authDBContext.Cooks.Remove( new Cook {BaseUserId = userId});
        else if (role == RoleType.Manager)
            _authDBContext.Managers.Remove( new Manager {BaseUserId = userId});
    }

    public async Task EditRestaurant(Guid restaurantId, string newName)
    {
        string name = newName.Trim();
        var restaurant = await _backendDBContext.Restaurants.FindAsync(restaurantId)
            ?? throw new BackendException(404, "Такого ресторана не существует");

        restaurant.Name = name;
        try
        {
            await _backendDBContext.SaveChangesAsync();
        }
        catch (Exception exc)
        {
            throw new BackendException(
                "Невозможно применить. Возможно ресторан с таким именем уже существует",
                exc
            );
        }
    }

    public async Task<Page<GenericItem>> GetRestaurants(int page, string? searchQuery)
    {
        
        var rests = await _restaurantService.GetRestaurants(page, searchQuery);
        Page<GenericItem> rests2 = new()
        {
            PageInfo = rests.PageInfo,
            Items = new List<GenericItem>()
        };
        foreach (var item in rests.Items)
        {
            rests2.Items.Add(item);
        }

        // rests2.Items = (ICollection<GenericItem>)rests.Items;
        return rests2;
    }

    public async Task<IEnumerable<AvailableRestaurant>> GetAvailableRestaurants()
    {
        return await _backendDBContext.Restaurants
            .Select(r => new AvailableRestaurant
            {
                Id = r.Id,
                Name = r.Name,
                UserWorkingHere = false
            }).ToListAsync();
    }

    public async Task DeleteRestaurant(Guid restaurantId)
    {
        var rest = await _backendDBContext.Restaurants
            .FindAsync(restaurantId)
                ?? throw new BackendException(404, "Запрашиваемый ресторан не существует");
            
        if ( !await AllOrdersAreCompleted(restaurantId) )
            throw new BackendException(400, "У ресторана остались невыполненные заказы, удаление невозможно");
        
        

        // повара и менджеры каскадно удалятся

        var cooksIds = _backendDBContext.Cooks
            .Where(c => c.RestaurantId == restaurantId)
            .Select(c => c.Id);
        var managersIds = _backendDBContext.Managers
            .Where(c => c.RestaurantId == restaurantId)
            .Select(c => c.Id);

        foreach (var cookId in cooksIds)
            await RemoveFromRole(cookId, RoleType.Cook);
        
        foreach (var managerId in managersIds)
            await RemoveFromRole(managerId, RoleType.Manager);

        _backendDBContext.Restaurants.Remove(rest);

        Task.WaitAll(new Task[] {
            _authDBContext.SaveChangesAsync(),
            _backendDBContext.SaveChangesAsync()
        });
            
    }


    public async Task CreateRestaurant(string name)
    {
        await _backendDBContext.Restaurants.AddAsync(
            new BackendDAL.Entities.Restaurant {Name = name}
        );
        await _backendDBContext.SaveChangesAsync();
    }

}
