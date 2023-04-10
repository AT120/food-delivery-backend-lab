using System.Net;
using Microsoft.AspNetCore.Authorization;
using ProjCommon.Enums;

namespace ProjCommon;

public static class Policies
{
    public const string RefreshOnly = "RefreshOnly";
    // public const string RefreshOnly = "RefreshOnly";


    public static void AddRefreshOnlyPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy(RefreshOnly, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimType.TokenType, TokenType.Refresh.ToString());
        });
    }

    public static void UpdateDefaultPolicy(this AuthorizationOptions options)
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(ClaimType.TokenType, TokenType.Access.ToString())
            .Build();
    }
}


