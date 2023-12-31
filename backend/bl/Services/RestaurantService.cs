using System.Data;
using BackendCommon.Const;
using BackendDAL;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Interfaces;

namespace BackendBl.Services;


public class RestaurantService : IRestaurantService
{
    private readonly BackendDBContext _dbcontext;
    public RestaurantService(BackendDBContext dc)
    {
        _dbcontext = dc;
    }

    public async Task<Page<Restaurant>> GetRestaurants(int page, string? searchQuery)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");
          
            
        var query = 
            _dbcontext.Restaurants
                .Select(r => new Restaurant 
                {
                    Id = r.Id,
                    Name = r.Name,
                });

        if (searchQuery is not null)
            query = query.Where(r => r.Name.Contains(searchQuery));

        int size = await query.CountAsync();
        PageInfo pageInfo = new (page, size, PageSize.Default);

        if (pageInfo.RangeStart > size)
            throw new BackendException(400, "Incorrect page number");     
        
        var restaurants = await query
                .OrderBy(x => x.Name)
                .Skip(pageInfo.RangeStart)
                .Take(PageSize.Default)
                .ToListAsync();
        

        return new Page<Restaurant> {
            PageInfo = pageInfo,
            Items = restaurants
        };
    }

    public async Task<ICollection<MenuDTO>> GetMenus(Guid restaurantId)
    {
        var restExists = await  _dbcontext.Restaurants.AnyAsync(x => x.Id == restaurantId);
        if (!restExists)
            throw new BackendException(404, "Requested restaurant does not exist.");

        return await _dbcontext.Menus
            .Where(m => m.RestaurantId == restaurantId && !m.Archived)
            .Select(m => new MenuDTO 
            {
                Id = m.Id,
                Name = m.Name,
            })
            .ToListAsync();
    }
}
