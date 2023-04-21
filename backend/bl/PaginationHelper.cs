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
        if (page.RangeStart == 1 && page.RangeEnd == page.Size)
            return 200;
        return 206;
    }
}