using BackendCommon.DTO;
using BackendCommon.Enums;

namespace BackendCommon.Interfaces;

public interface IDishService
{
    Task<DishesPage> GetDishes(
        Guid restaurantId,
        int page,
        bool vegetarianOnly,
        IEnumerable<int>? menus,
        IEnumerable<DishCategory>? categories,
        SortingTypes sorting);
}