using System.Security.Claims;
using AuthCommon.DTO;

namespace AuthCommon.Interfaces;

public interface IProfileService
{
    Task<UserProfile> GetUserProfile(ClaimsPrincipal userPrincipal);
}