using System.Security.Claims;
using BackendCommon.DTO;

namespace BackendCommon.Interfaces;

public interface ICartService
{
    Task<Cart> GetCart(Guid userId);
    Task PutDishIntoCart(DishCount dishModel, Guid userId);
    Task ChangeDishQunatity(Guid dishId, int count, Guid userId);
    Task DeleteDishFromCart(Guid dishId, Guid userId);
    Task CleanCart(Guid userId);
}