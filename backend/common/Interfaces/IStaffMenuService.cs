using BackendCommon.DTO;
using ProjCommon.DTO;

namespace BackendCommon.Interfaces;

public interface IStaffMenuService
{
    Task<Page<MenuShort>> GetMenus(int page, bool? archived, Guid managerId);
    Task<int> CreateMenu(string name, IEnumerable<Guid>? dishes, Guid managerId);
    Task EditMenu(int menuId, bool? archived, string? name, Guid managerId);
    Task DeleteMenu(int menuId, Guid managerId);
    Task<MenuDetailed> GetMenuDetailed(int menuId, Guid managerId);
    Task AddDishToMenu(int menuId, Guid dishId, Guid managerId);
    Task DeleteDishFromMenu(int menuId, Guid dishId, Guid managerId);
}