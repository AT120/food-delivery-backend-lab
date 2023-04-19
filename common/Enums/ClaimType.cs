using System.Data;
using System.Reflection.Metadata;

namespace ProjCommon.Enums;

public static class ClaimType
{
    public const string TokenType = "tokenType";
    public const string TokenId = "tokenId";
    public const string Role = "role";
    public const string Address = "address";
    public const string UserId = "id";

    // CanOrder,
    // CanCancelOrder,
    // CanCookAndPackage,
    // CanDeliver,
    // CanSeeAllOrders,
}