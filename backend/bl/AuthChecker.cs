using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProjCommon.Enums;

namespace BackendBl;

public static class AuthenticationChecker
{
    public static Task UserIdChecker(TokenValidatedContext context)
    {
        try 
        {
            var res = context.Principal.HasClaim(x => x.Type == ClaimType.UserId);
            if (!res)
                context.Fail("Provided token has been invalidated.");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            context.Fail(ex);
            throw;
        }
    }
}