using System.Security.Claims;
using BackendCommon.DTO;
using BackendCommon.Enums;

namespace BackendCommon.Interfaces;

public interface IDishService
{
    Task<Page<DishShort>> GetDishes(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting);
    
    Task<DishDetailed> GetDish(Guid dishId, ClaimsPrincipal user);
    Task RateDish(Guid dishId, ClaimsPrincipal userPrincipal, int newRating);
}