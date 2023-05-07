using ProjCommon.DTO;
using ProjCommon.Enums;

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
        List<RoleType> rolestmp = new();
        for (int i = 0; i < 5; i++) //TODO:max roletype
            if (dto.Roles[i])
                rolestmp.Add((RoleType)i);
        Roles = rolestmp;
    }
}