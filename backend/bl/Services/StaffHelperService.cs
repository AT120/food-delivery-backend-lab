using BackendCommon.Interfaces;
using BackendDAL;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Exceptions;

namespace BackendBl.Services;

public class StaffHelperService : IStaffHelperService
{
    private readonly BackendDBContext _dbcontext;
    public StaffHelperService(BackendDBContext dbc)
    {
        _dbcontext = dbc;
    }

    public async Task<Guid> GetManagerRestaurant(Guid userId)
    {
        var manager = await _dbcontext.Managers.FindAsync(userId)
            ?? throw new BackendException(404, "User is not registered as a manager");
        return manager.RestaurantId;
    }


    public async Task<bool> CheckMenuExistence(Guid restaurantId, int menuId)
    {
        return await _dbcontext.Menus
            .AnyAsync(m => m.Id == menuId && m.RestaurantId == restaurantId);
    }


    public async Task<bool> CheckDishExistence(Guid restaurantId, Guid dishId)
    {
        return await _dbcontext.Dishes
            .AnyAsync(m => m.Id == dishId && m.RestaurantId == restaurantId);
    }
}