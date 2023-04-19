using System.Security.Claims;
using BackendCommon.DTO;

namespace BackendCommon.Interfaces;

public interface ICartService
{
    Task<CartDTO> GetCart(ClaimsPrincipal user);
    Task PutDishIntoCart(DishCount dishModel, ClaimsPrincipal user);
    Task ChangeDishQunatity(Guid dishId, ClaimsPrincipal user, int count);
    Task DeleteDishFromCart(Guid dishId, ClaimsPrincipal user);
    Task CleanCart(ClaimsPrincipal user);
}