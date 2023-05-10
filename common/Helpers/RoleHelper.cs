using ProjCommon.Enums;

namespace ProjCommon.Helpers;

public static class RoleHelper
{
    public static List<RoleType> GetRolesList(bool[] roles)
    {
        List<RoleType> rolesList = new();
        for (int i = 0; i < 5; i++) //TODO:max roletype
            if (roles[i])
                rolesList.Add((RoleType)i);
        return rolesList;
    }
}