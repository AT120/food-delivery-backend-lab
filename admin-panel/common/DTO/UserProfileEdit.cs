using ProjCommon.DTO;
using ProjCommon.Enums;
using ProjCommon.Helpers;

namespace AdminCommon.DTO;

public class UserProfileEdit
{
    public Guid UserId { get; set; }
    public IEnumerable<RoleType> Roles { get; set; }
    public Guid? NewRestaurantId { get; set; } 

    public UserProfileEdit(UserProfileEditDTO dto)
    {
        UserId = dto.UserId;
        NewRestaurantId = dto.NewRestaurantId;
        Roles = RoleHelper.GetRolesList(dto.Roles);
    }
}