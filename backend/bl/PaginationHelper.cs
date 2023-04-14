using BackendCommon.DTO;

namespace BackendBl;

public static class PaginationHelper
{
    public static string FillContentRange(PageInfo page)
    {
        return $"restaurants {page.RangeStart}-{page.RangeEnd}/{page.Size}";
    }

    public static int GetStatusCode(PageInfo page)
    {
        return ((page.RangeEnd - page.RangeStart) < page.Size) ? 206 : 200;
    }
}