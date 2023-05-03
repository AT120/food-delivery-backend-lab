using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AdminCommon;

public static class QueryHelper
{
    public static string GetSearchQuery(this HttpContext context)
    {
        var sqvalues = context.Request.Query["searchQuery"];
        return (sqvalues == StringValues.Empty) ? "" : sqvalues[0];
    }
}