using BackendCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IRestaurantService
{
    Task<Page<RestaurantDTO>> GetRestaurants(int page, string? searchQuery);
    Task<ICollection<MenuDTO>> GetMenus(Guid restaurantId);
}