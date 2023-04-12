using System.Data;
using System.Formats.Tar;
using BackendCommon.Const;
using BackendCommon.DTO;
using BackendCommon.Interfaces;
using BackendDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Exceptions;

namespace BackendBl.Services;


public class RestaurantService : IRestaurantService
{
    private readonly BackendDBContext _dbcontext;
    public RestaurantService(BackendDBContext dc)
    {
        _dbcontext = dc;
    }

    public async Task<RestaurantsPage> GetRestaurants(int page, string? searchQuery)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");
        
        int size = await _dbcontext.Restaurants.CountAsync();
        int rangeStart = PageSize.Default * (page-1);
        int rangeEnd = Math.Min(rangeStart + PageSize.Default, size);

        if (rangeStart > size)
            throw new BackendException(400, "Incorrect page number");           
            
        var query = 
            _dbcontext.Restaurants
                .Select(r => new RestaurantDTO 
                {
                    Id = r.Id,
                    Name = r.Name,
                });

        if (searchQuery is not null)
            query = query.Where(r => r.Name.Contains(searchQuery));
        
        var restaurants = await query
                .Skip(rangeStart)
                .Take(PageSize.Default)
                .ToListAsync();
        
        return new RestaurantsPage {
            RangeStart = rangeStart,
            RangeEnd = rangeEnd,
            Size = size,
            Restaurants = restaurants
        };
    }
}
