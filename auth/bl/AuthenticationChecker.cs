using AuthDAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjCommon;
using ProjCommon.Enums;

namespace AuthBL;

public static class AuthenticationChecker
{
    public async static Task RefreshChecker(TokenValidatedContext context)
    {
        try 
        {
            TokenType type 
                = ClaimsHelper.GetValue<TokenType>(ClaimType.TokenType, context.Principal);

            if (type == TokenType.Access)
                return;

            int id = ClaimsHelper.GetValue<int>(ClaimType.TokenId, context.Principal);

            var dbcontext = context.HttpContext
                .RequestServices
                .GetRequiredService<AuthDBContext>();
            
            if (! await dbcontext.Tokens.AnyAsync(t => t.Id == id))
                context.Fail("Provided token has been invalidated.");
                return;
        }
        catch (Exception ex)
        {
            context.Fail(ex);
            throw;
        }
    }
}