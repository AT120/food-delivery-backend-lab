using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthCommon.DTO;
using AuthCommon.Interfaces;
using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace AuthBL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AuthDBContext _dbcontext;
    private readonly RoleManager<Role> _roleManager;

    public AuthService(
        UserManager<User> um,
        AuthDBContext dbc,
        RoleManager<Role> rm)
    {
        _userManager = um;
        _dbcontext = dbc;
        _roleManager = rm;
    }


    private async Task<IList<Claim>> GetUserClaims(User user) 
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(ClaimsHelper.CreateClaim(ClaimType.Role, role));
        }
        claims.Add(ClaimsHelper.CreateClaim(ClaimType.UserId, user.Id));

        return claims;
    } 


    private async Task<string> GenerateAccessToken(User user)
    {

        var claims = await GetUserClaims(user);
        claims.Add(ClaimsHelper.CreateClaim(ClaimType.TokenType, TokenType.Access));

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: TokenConfiguration.Issuer,
            notBefore: now,
            expires: now.AddSeconds(TokenConfiguration.AccessTokenLifetime),
            claims: claims,
            signingCredentials: new SigningCredentials(
                TokenConfiguration.Key,
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private static string GenerateRefreshToken(User user, int tokenId)
    {

        var claims = new List<Claim>
        {
            ClaimsHelper.CreateClaim(ClaimType.UserId, user.Id),
            ClaimsHelper.CreateClaim(ClaimType.TokenType, TokenType.Refresh),
            ClaimsHelper.CreateClaim(ClaimType.TokenId, tokenId)
        };

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: TokenConfiguration.Issuer,
            notBefore: now,
            expires: now.AddSeconds(TokenConfiguration.RefreshTokenLifetime),
            claims: claims,
            signingCredentials: new SigningCredentials(
                TokenConfiguration.Key,
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<TokenPair> GenerateTokenPair(User user)
    {
        var tokenDB = new IssuedToken 
        {
            UserId = user.Id,
            ValidUntil = DateTime.UtcNow.AddSeconds(TokenConfiguration.RefreshTokenLifetime)
        };

        await _dbcontext.Tokens.AddAsync(tokenDB);
        await _dbcontext.SaveChangesAsync();


        var tokens = new TokenPair
        {
            RefreshToken = GenerateRefreshToken(user, tokenDB.Id),
            AccessToken = await GenerateAccessToken(user)
        };

        return tokens;
    }


    public async Task<TokenPair> Register(RegisterCreds creds)
    {
        var user = new User 
        {
            UserName = creds.Email,
            Email = creds.Email,
            FullName = creds.Name,
            PhoneNumber = creds.PhoneNumber,
            Gender = creds.Gender,
            BirthDate = creds.BirthDate,
        };
        var res = await _userManager.CreateAsync(user, creds.Password);
        if (!res.Succeeded)
            throw new BackendException(400, res.Errors.First().Description);

        res = await _userManager.AddToRoleAsync(user, Enum.GetName(RoleType.Customer));
        if (!res.Succeeded)
            throw new BackendException(
                500,
                "An error occured while creating the user",
                $"Failed to add user to role {res.Errors}"
            );

        await _dbcontext.Customers.AddAsync(new Customer
        {
            Address = creds.Address,
            BaseUser = user,
            Id = user.Id
        });
        
        await _dbcontext.SaveChangesAsync();
        return await GenerateTokenPair(user);
    }


    public async Task<TokenPair> Login(LoginCreds creds)
    {
        User user = await _userManager.FindByEmailAsync(creds.Email)
            ?? throw new BackendException(401, "Login or password is incorrect.");

        var res = await _userManager.CheckPasswordAsync(user, creds.Password);
        if (!res)
            throw new BackendException(401, "Login or password is incorrect.");

        return await GenerateTokenPair(user);
    }

    public async Task Logout(Guid userId)
    {
        await _dbcontext.Tokens
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task Logout(int tokenId)
    {
        await _dbcontext.Tokens
            .Where(t => t.Id == tokenId)
            .ExecuteDeleteAsync();
    }

    public async Task<TokenPair> Refresh(ClaimsPrincipal userPrincipal)
    {
        User user = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new BackendException(404, "Requested user does not exist.");
        return await GenerateTokenPair(user);
    }

    public async Task ChangePassword(PasswordPair passwords, ClaimsPrincipal userPrincipal)
    {
        User user = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new BackendException(404, "Requested user does not exist.");
        
        var res = await _userManager.ChangePasswordAsync(
            user,
            passwords.OldPassword,
            passwords.NewPassword
        );

        if (!res.Succeeded) {
            throw new BackendException(400, res.Errors.First().Description);
        }
    }
}