using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProjCommon.Enums;

namespace ProjCommon;

public static class ClaimsHelper
{
    private readonly static Dictionary<string, Func<string, object>> _converters = new()
    {
        {ClaimType.Address, x => x},
        {ClaimType.UserId, x => Guid.Parse(x)},
        {ClaimType.Role, x => x},
        {ClaimType.TokenId, x => int.Parse(x)},
        {ClaimType.TokenType, x => Enum.Parse<TokenType>(x)}
    };

    public static Claim CreateClaim(string type, object value)
    {
        string val = value.ToString() ?? throw new ArgumentException("Can't parse your object.");
        return new Claim(type, val);
    }


    public static T GetValue<T>(string type, ClaimsPrincipal user) 
    {
        var converter = _converters[type]
            ?? throw new ArgumentException("There is no converter for specified type");

        string value = user.FindFirstValue(type)
            ?? throw new ArgumentException("There is no claim of such type in specified principal.");
        
        return (T)converter(value)
            ?? throw new ArgumentException("Can't cast claim to specified type.");
    }

    public static Guid GetUserId(ClaimsPrincipal user) =>
        GetValue<Guid>(ClaimType.UserId, user);
}