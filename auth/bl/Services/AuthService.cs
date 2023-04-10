using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthCommon.DTO;
using AuthCommon.Interfaces;
using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
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


    private async Task CreateCustomer(User user, string address)
    {
        var res = await _userManager.AddClaimAsync(
            user,
            ClaimsHelper.CreateClaim(ClaimType.Address, address)
        );
        if (!res.Succeeded)
            throw new BackendException(
                500,
                "An error occured while creating the user",
                $"Failed to create claim {res.Errors}"
            );
        res = await _userManager.AddToRoleAsync(user, Enum.GetName(RoleType.Customer));
        if (!res.Succeeded)
            throw new BackendException(
                500,
                "An error occured while creating the user",
                $"Failed to add user to role {res.Errors}"
            );

        await _dbcontext.Customers.AddAsync(new Customer
        {
            Address = address,
            BaseUser = user,
            Id = user.Id
        });

        await _dbcontext.SaveChangesAsync();
    }


    private async Task<IList<Claim>> GetUserClaims(User user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(ClaimsHelper.CreateClaim(ClaimType.Role, role));
        }
        claims.Add(ClaimsHelper.CreateClaim(ClaimType.Id, user.Id));

        return claims;
    } 


    private async Task<string> GenerateToken(User user, TokenType type)
    {

        var claims = await GetUserClaims(user);
        claims.Add(ClaimsHelper.CreateClaim(ClaimType.TokenType, type));
        int lifetime = (type == TokenType.Refresh)
             ? TokenConfiguration.RefreshTokenLifetime
             : TokenConfiguration.AccessTokenLifetime;
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: TokenConfiguration.Issuer,
            // issuer: "Rayman",
            notBefore: now,
            expires: now.AddMinutes(lifetime),
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
        var tokens = new TokenPair
        {
            RefreshToken = await GenerateToken(user, TokenType.Refresh),
            AccessToken = await GenerateToken(user, TokenType.Access)
        };

        // _dbcontext.IssuedTokens.Add(
        //     new IssuedTokenDbModel { RefreshTokenId = parentRefreshTokenId }
        // );
        // _dbcontext.SaveChanges();

        return tokens;
    }


    public async Task<TokenPair> Register(RegisterCreds creds)
    {
        var user = new User(creds);
        var res = await _userManager.CreateAsync(user, creds.Password);
        if (!res.Succeeded)
            throw new BackendException(400, res.Errors.First().Description);
        if (creds.Address is not null)
            await CreateCustomer(user, creds.Address);

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
}