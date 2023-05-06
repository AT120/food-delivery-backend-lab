using AdminCommon.Interfaces;
using BackendDAL;
using Microsoft.AspNetCore.Server.IIS.Core;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Interfaces;

namespace AdminBL.Services;

public class AdminRestaurantService : IAdminRestaurantService
{
    private readonly IRestaurantService _restaurantService;
    private readonly BackendDBContext _backendDBContext;

    public AdminRestaurantService(IRestaurantService rs, BackendDBContext bdb)
    {
        _backendDBContext = bdb;
        _restaurantService = rs;
    }

    public async Task EditRestaurant(Guid restaurantId, string newName)
    {
        var restaurant = await _backendDBContext.Restaurants.FindAsync(restaurantId)
            ?? throw new BackendException("test");
        
    }

    public async Task<Page<GenericItem>> GetRestaurants(int page, string? searchQuery)
    {
        
        var rests = await _restaurantService.GetRestaurants(page, searchQuery);
        Page<GenericItem> rests2 = new();
        rests2.PageInfo = rests.PageInfo;
        rests2.Items = new List<GenericItem>();
        foreach (var item in rests.Items)
        {
            rests2.Items.Add(item);
        }

        // rests2.Items = (ICollection<GenericItem>)rests.Items;
        return rests2;
    }


}
