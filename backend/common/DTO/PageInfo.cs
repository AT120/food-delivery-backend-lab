namespace BackendCommon.DTO;

public struct PageInfo
{
    public PageInfo()
    {
    }

    public int RangeStart { get; set; } = 0;
    public int RangeEnd { get; set; } = 0;
    public int Size { get; set; } = 0;
}
