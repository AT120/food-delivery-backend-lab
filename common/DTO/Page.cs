namespace ProjCommon.DTO;

public struct PageInfo
{
    public PageInfo(int rangeStart, int rangeEnd, int size)
    {
        RangeStart = (size == 0) ? 0 : rangeStart + 1;
        RangeEnd = rangeEnd;
        Size = size;
    }

    public int RangeStart { get; set; } = 0;
    public int RangeEnd { get; set; } = 0;
    public int Size { get; set; } = 0;
}


public class Page<T>
{
    public PageInfo PageInfo { get; set; }
    public ICollection<T> Items { get; set; }

    public static implicit operator Page<T>(Page<Restaurant> v)
    {
        throw new NotImplementedException();
    }
}