namespace BackendCommon.Interfaces;

public interface IStaffHelperService
{
    Task<Guid> GetManagerRestaurant(Guid userId);
    Task<bool> CheckMenuExistence(Guid restaurantId, int menuId);
    Task<bool> CheckDishExistence(Guid restaurantId, Guid dishId);
}