using System.Reflection.Metadata;

namespace ProjCommon.Enums;

public static class ClaimType
{
    public static readonly string TokenType = "tokenType";
    public static readonly string TokenId = "tokenId";
    public static readonly string Role = "role";
    public static readonly string Address = "address";
    public static readonly string Id = "id";

    // CanOrder,
    // CanCancelOrder,
    // CanCookAndPackage,
    // CanDeliver,
    // CanSeeAllOrders,
}