using AuthCommon.DTO;

namespace AuthCommon.Interfaces;

public interface IAuthService
{
    Task<TokenPair> Register(RegisterCreds creds);
    Task<TokenPair> Login(LoginCreds creds);
    Task Logout(Guid userId);
    Task Logout(int tokenId);
}