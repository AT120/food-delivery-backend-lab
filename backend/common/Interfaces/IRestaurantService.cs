using BackendCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IRestaurantService
{
    Task<RestaurantsPage> GetRestaurants(int page, string? searchQuery);
    
}