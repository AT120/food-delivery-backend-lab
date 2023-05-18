namespace AdminCommon.Interfaces;

public interface IAdminAuthService
{
    Task SignIn(string Email, string Password);
    Task SignOut();
}