using ProjCommon.DTO;

namespace ProjCommon.Interfaces;

public interface IRestaurantService
{
    Task<Page<Restaurant>> GetRestaurants(int page, string? searchQuery);
    Task<ICollection<MenuDTO>> GetMenus(Guid restaurantId);
}