using AuthDAL.Models;
using ProjCommon;
using ProjCommon.DTO;

namespace AdminBL;

public static class Converter
{
    public static IEnumerable<string> GetStringRoles(IEnumerable<Role> roles) 
        => roles.Select(u => u.Name);

    public static UserProfile GetUserProfile(User user)
    {
        return new UserProfile
        {
            BirthDate = user.BirthDate,
            Email = user.Email,
            FullName = user.FullName,
            Gender = user.Gender,
            Roles = GetStringRoles(user.Roles),
            PhoneNumber = user.PhoneNumber,
            Address = null, //TODO
        };
    }
}