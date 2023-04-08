using AuthCommon.DTO;

namespace AuthCommon.Interfaces;

public interface IAuthService
{
    Task Register(RegisterCreds creds);
}