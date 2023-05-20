using Microsoft.AspNetCore.Builder;
using ProjCommon.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProjCommon.Enums;

namespace ProjCommon.Configurators;

public static class TokenConfiguration
{
    public static string? Issuer { get; private set; }
    public static string? Authority { get; private set; }
    public static int AccessTokenLifetime { get; private set; }
    public static int RefreshTokenLifetime { get; private set; }
    public static SymmetricSecurityKey? Key { get; private set; }
    public static string? SecurityKey { get; private set; }

    public static void ConfigureToken(this WebApplicationBuilder builder)
    {
        var tokencfg = builder.Configuration.GetSection("TokenCfg")
            ?? throw new InvalidConfigException("'TokenCfg' section was not found in your config.");

        Issuer = tokencfg["Issuer"] 
            ?? throw new InvalidConfigException("'Issuer' was not found in your config.");

        // Authority = tokencfg["Authority"] 
        //     ?? throw new InvalidConfigException("'Issuer' was not found in your config.");

        AccessTokenLifetime = Int32.Parse(
            tokencfg["AccessTokenLifetime"]
                ?? throw new InvalidConfigException("'AccessTokenLifetime' was not found in your config.")
        );

        RefreshTokenLifetime = Int32.Parse(
            tokencfg["RefreshTokenLifetime"]
                ?? throw new InvalidConfigException("'RefreshTokenLifetime' was not found in your config.")
        );

        SecurityKey = tokencfg["SecurityKey"]
                    ?? throw new InvalidConfigException("'SecurityKey' was not found in your config.");

        Key = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(SecurityKey)
        );

        if (Key.KeySize < 128)
            throw new InvalidConfigException("Your 'SecurityKey' is too short.");
        
    }


    public static void AddJwtAuthentication(this IServiceCollection services, JwtBearerEvents? events = null)
    {
        if (Issuer is null)
            throw new InvalidOperationException("Token configuration is empty. You have to call ConfigureToken first.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = Key,
                    NameClaimType = ClaimType.UserId,
                    // RoleClaimType = ClaimType.Role,
                };

                if (events is not null)
                    opts.Events = events;
            });
    }
}