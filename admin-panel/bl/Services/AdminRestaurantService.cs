using AdminCommon.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjCommon.DTO;
using ProjCommon.Interfaces;

namespace AdminBL.Services;

public class AdminRestaurantService : IAdminRestaurantService
{
    private readonly IRestaurantService _restaurantService;
    public AdminRestaurantService(IRestaurantService rs)
    {
        _restaurantService = rs;
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
