using System.Security.Claims;
using ProjCommon.Enums;

namespace AuthBL;

public class ClaimsHelper
{
    public static Claim CreateClaim(string type, object value)
    {
        string val = value.ToString() ?? throw new ArgumentException("Can't parse your object.");
        return new Claim(type, val);
    }
}