using System.Security.Claims;
using AuthCommon.DTO;
using ProjCommon.DTO;

namespace AuthCommon.Interfaces;

public interface IProfileService
{
    Task<UserProfile> GetUserProfile(ClaimsPrincipal userPrincipal);
    Task UpdateUserProfile(UserProfileEdit newProfile, ClaimsPrincipal userPrincipal);
}