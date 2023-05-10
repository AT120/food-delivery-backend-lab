using System.ComponentModel.DataAnnotations;
using AuthCommon.DTO;
using ProjCommon.Enums;
using ProjCommon.Helpers;

namespace AdminCommon.DTO;

public class UserProfileCreate
{
    public RegisterUserData UserData { get; set; }
    public Guid? ResturantId { get; set; }
    public IEnumerable<RoleType> Roles { get; set; }    


    public UserProfileCreate(UserProfileCreateDTO dto)
    {
        UserData = dto.UserData;
        ResturantId = dto.RestaurantId;
        Roles = RoleHelper.GetRolesList(dto.Roles);
    }
}