using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ProjCommon.DTO;

namespace ProjCommon.Helpers;

public static class PaginationHelper
{
    public static string FillContentRange(PageInfo page)
    {
        return $"restaurants {page.RangeStart}-{page.RangeEnd}/{page.Size}";
    }

    public static int GetStatusCode(PageInfo page)
    {
        if (page.RangeStart == 1 && page.RangeEnd == page.Size)
            return 200;
        return 206;
    }

    public static int? GetCurrentPage(this HttpContext context)
    {
        var pageParameter = context.Request.Query["page"];
        if (pageParameter == StringValues.Empty)
            return null;

        var success = int.TryParse(pageParameter[0], out int page);
        return  (success) ? page : null;
    }
}