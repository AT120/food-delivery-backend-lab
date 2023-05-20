using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProjCommon.Helpers;

public static class TokenHelper
{

    public static Task TokenFromQuery(MessageReceivedContext context)
    {
        var accessToken = context.Request.Query["access_token"];

        var path = context.HttpContext.Request.Path;
        if (!string.IsNullOrEmpty(accessToken) &&
            path.StartsWithSegments("/notifications"))
        {
            context.Token = accessToken;
        }
        return Task.CompletedTask;   
        
    }
}