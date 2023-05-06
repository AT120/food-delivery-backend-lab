using ProjCommon.DTO;
using ProjCommon.Enums;

namespace AdminCommon.Interfaces;

public interface IAdminUserService
{
    Task<Page<UserProfile>> GetUsers(
        int page,
        string? nameSearchQuery = null,
        string? emailSearchQuery = null,
        string? phoneSearchQuery = null,
        Gender? gender = null,
        IEnumerable<RoleType>? roles = null
    );
}