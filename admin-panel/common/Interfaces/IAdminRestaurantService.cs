using ProjCommon.DTO;

namespace AdminCommon.Interfaces;

public interface IAdminRestaurantService
{
    Task<Page<GenericItem>> GetRestaurants(int page, string? searchQuery);
    Task EditRestaurant(Guid restaurantId, string newName);
    
}