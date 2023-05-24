using BackendCommon.DTO;
using BackendCommon.Enums;
using ProjCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IDishService
{
    Task<Page<DishShort>> GetDishesCustomer(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting);
    
    Task<Page<DishShort>> GetDishesManager(
        Guid managerId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting,
        bool? archived);
    
    Task<DishDetailed> GetDish(Guid dishId, Guid? userId);
    Task RateDish(Guid dishId, int newRating, Guid userId);
    Task EditDish(Guid dishId, Guid managerId, DishEdit newDish);
    Task<Guid> CreateDish(Guid managerId, DishCreate dish);
}