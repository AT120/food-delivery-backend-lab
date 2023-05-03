using BackendCommon.DTO;
using BackendCommon.Enums;
using ProjCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IDishService
{
    Task<Page<DishShort>> GetDishes(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting
    );
    
    Task<DishDetailed> GetDish(Guid dishId, Guid? userId);
    Task RateDish(Guid dishId, int newRating, Guid userId);
}