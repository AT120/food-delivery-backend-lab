using AdminCommon.DTO;
using AuthDAL.Models;
using ProjCommon.DTO;
using ProjCommon.Enums;

namespace AdminBL;

public static class Converter
{
    public static IEnumerable<RoleType> GetRoleTypes(IEnumerable<Role> roles) 
        => roles.Select(u => u.RoleType);

    public static UserProfile GetUserProfile(User user)
    {
        return new UserProfile
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Gender = user.Gender,
            Roles = GetRoleTypes(user.Roles),
            PhoneNumber = user.PhoneNumber,
        };
    }

    public static UserProfileDetailed GetUserProfileDetailed(
        User user,
        BackendDAL.Entities.Restaurant? restaurant = null)
    {
        return new UserProfileDetailed
        {
            Id = user.Id,
            BirthDate = user.BirthDate,
            Email = user.Email,
            FullName = user.FullName,
            Gender = user.Gender,
            Restaurant = (restaurant == null) ? null : new GenericItem {Id = restaurant.Id, Name = restaurant.Name},
            PhoneNumber = user.PhoneNumber,
            Roles = GetRoleTypes(user.Roles)
        };
    }
}